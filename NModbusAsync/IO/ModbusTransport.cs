using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbusAsync.Messages;

namespace NModbusAsync.IO
{
    internal abstract class ModbusTransport : IModbusTransport
    {
        private readonly SemaphoreSlim semaphoreSlim;

        private int waitToRetryMilliseconds;

        protected ModbusTransport(IPipeResource pipeResource, IModbusLogger logger)
        {
            PipeResource = pipeResource ?? throw new ArgumentNullException(nameof(pipeResource));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public int ReadTimeout { get => PipeResource.ReadTimeout; set => PipeResource.ReadTimeout = value; }

        public int WriteTimeout { get => PipeResource.WriteTimeout; set => PipeResource.WriteTimeout = value; }

        public int Retries { get; set; }

        public uint RetryOnOldResponseThreshold { get; set; }

        public bool SlaveBusyUsesRetryCount { get; set; }

        public int WaitToRetryMilliseconds
        {
            get
            {
                return waitToRetryMilliseconds;
            }

            set
            {
                if (value < Timeout.Infinite)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The value needs to be either -1 (signifying an infinite timeout), 0 or a positive integer");
                }

                waitToRetryMilliseconds = value;
            }
        }

        public IPipeResource PipeResource { get; }

        protected IModbusLogger Logger { get; }

        public async Task<TResponse> SendAsync<TResponse>(IModbusRequest request, CancellationToken token = default)
            where TResponse : IModbusResponse, new()
        {
            IModbusResponse response = null;
            int attempt = 1;
            bool success = false;

            do
            {
                try
                {
                    await semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
                    try
                    {
                        await WriteRequestAsync(request, token).ConfigureAwait(false);

                        bool readAgain;
                        do
                        {
                            readAgain = false;
                            response = await ReadResponseAsync<TResponse>(token).ConfigureAwait(false);

                            if (response is SlaveExceptionResponse exceptionResponse)
                            {
                                // if SlaveExceptionCode == ACKNOWLEDGE we retry reading the response without resubmitting request
                                readAgain = exceptionResponse.SlaveExceptionCode == SlaveExceptionCode.Acknowledge;

                                if (readAgain)
                                {
                                    LogAcknowledgeResponse(request);
                                    await Task.Delay(WaitToRetryMilliseconds, token).ConfigureAwait(false);
                                }
                                else
                                {
                                    throw new SlaveException(exceptionResponse);
                                }
                            }
                            else if (RetryReadResponse(request, response))
                            {
                                readAgain = true;
                            }
                        }
                        while (readAgain);
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }

                    Validate(request, response);
                    success = true;
                }
                catch (SlaveException se)
                {
                    if (se.SlaveExceptionCode != SlaveExceptionCode.SlaveDeviceBusy || (SlaveBusyUsesRetryCount && attempt++ > Retries))
                    {
                        throw;
                    }

                    LogSlaveDeviceBusyResponse(request);
                    await Task.Delay(WaitToRetryMilliseconds, token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex.InnerException is SocketException)
                    {
                        LogSocketException(request, ex);
                        throw;
                    }
                    else if (ex is FormatException || ex is IOException || ex is TimeoutException)
                    {
                        LogException(request, ex, attempt);
                        if (attempt++ > Retries)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            while (!success);

            return (TResponse)response;
        }

        public void Dispose()
        {
            PipeResource.Dispose();
            semaphoreSlim.Dispose();
        }

        protected abstract Task WriteRequestAsync(IModbusRequest request, CancellationToken token = default);

        protected abstract Task<IModbusResponse> ReadResponseAsync<TResponse>(CancellationToken token = default) where TResponse : IModbusResponse, new();

        protected abstract bool RetryReadResponse(IModbusRequest request, IModbusResponse response);

        protected abstract void Validate(IModbusRequest request, IModbusResponse response);

        private void LogAcknowledgeResponse(IModbusRequest request)
        {
            var message = $@"Received slave exception code 'Acknowledge' while sending request.
Waiting {WaitToRetryMilliseconds} milliseconds and retrying to read response.
Request: {request}.";
            Logger.Log(LogLevel.Debug, message);
        }

        private void LogSlaveDeviceBusyResponse(IModbusRequest request)
        {
            var message = $@"Received slave exception code 'SlaveDeviceBusy' while sending request.
Waiting {WaitToRetryMilliseconds} milliseconds and resubmitting request.
Request: {request}.";
            Logger.Log(LogLevel.Warning, message);
        }

        private void LogSocketException(IModbusRequest request, Exception ex)
        {
            var message = $@"Socket error occured while sending request.
Request: {request}
Exception: {ex.GetType().Name}";
            Logger.Log(LogLevel.Error, message);
        }

        private void LogException(IModbusRequest request, Exception ex, int attempt)
        {
            var message = $@"Error occured while sending request.
{Retries - attempt + 1} retries remaining.
Request: {request}
Exception: {ex.GetType().Name}";
            Logger.Log(LogLevel.Error, message);
        }
    }
}