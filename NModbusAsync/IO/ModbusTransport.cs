using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NModbusAsync.IO.Abstractions;
using NModbusAsync.Messages;

namespace NModbusAsync.IO
{
    internal abstract class ModbusTransport : IModbusTransport
    {
        private readonly ITransactionIdProvider transactionIdProvider;
        private readonly SemaphoreSlim semaphoreSlim;
        private readonly CancellationTokenSource disposingTokenSource;

        private int waitToRetryMilliseconds;

        protected ModbusTransport(IPipeResource pipeResource, ITransactionIdProvider transactionIdProvider, ILogger<IModbusMaster> logger)
        {
            PipeResource = pipeResource;
            Logger = logger;

            this.transactionIdProvider = transactionIdProvider;

            semaphoreSlim = new SemaphoreSlim(1, 1);
            disposingTokenSource = new CancellationTokenSource();
        }

        public int ReadTimeout { get => PipeResource.ReadTimeout; set => PipeResource.ReadTimeout = value; }

        public int WriteTimeout { get => PipeResource.WriteTimeout; set => PipeResource.WriteTimeout = value; }

        public uint Retries { get; set; }

        public uint RetryOnOldTransactionIdThreshold { get; set; }

        public uint RetryOnInvalidResponseCount { get; set; }

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
                    throw new ArgumentOutOfRangeException(nameof(value), "The value needs to be either -1 (signifying an infinite timeout), 0 or a positive integer.");
                }

                waitToRetryMilliseconds = value;
            }
        }

        public IPipeResource PipeResource { get; }

        protected ILogger<IModbusMaster> Logger { get; }

        public async Task<TResponse> SendAsync<TResponse>(IModbusRequest request, CancellationToken token = default)
            where TResponse : IModbusResponse, new()
        {
            ThrowIfDisposed();

            var attempt = 1;
            var success = false;

            using var currCallTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, disposingTokenSource.Token);
            token = currCallTokenSource.Token;

            IModbusResponse response = null;
            do
            {
                try
                {
                    await semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
                    try
                    {
                        request.TransactionId = transactionIdProvider.NewId();
                        Logger.LogTrace("Sending modbus request. TransactionId: {TransactionId}, Request: {Request}.", request.TransactionId, request);

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
                                    Logger.LogDebug(
                                        "Received slave exception code 'Acknowledge' while sending request. Waiting {WaitToRetryMilliseconds} milliseconds and retrying to read response. Request: {Request}.",
                                        WaitToRetryMilliseconds,
                                        request);

                                    await DelayRetryWithExceptionHandling(token).ConfigureAwait(false);
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
                catch (ObjectDisposedException)
                {
                    ThrowIfDisposed();
                    throw; // else throw original ObjectDisposedException
                }
                catch (OperationCanceledException)
                {
                    ThrowIfDisposed();
                    throw; // else throw original OperationCanceledException
                }
                catch (SlaveException se)
                {
                    if (se.SlaveExceptionCode != SlaveExceptionCode.SlaveDeviceBusy || (SlaveBusyUsesRetryCount && attempt++ > Retries))
                    {
                        throw;
                    }

                    Logger.LogWarning(
                        "Received slave exception code 'SlaveDeviceBusy' while sending request. Waiting {WaitToRetryMilliseconds} milliseconds and resubmitting request. Request: {Request}.",
                        WaitToRetryMilliseconds,
                        request);

                    await DelayRetryWithExceptionHandling(token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (ex is SocketException || ex.InnerException is SocketException)
                    {
                        Logger.LogError(ex, "Socket error occured while sending request. Request: {Request}.", request);
                        throw;
                    }
                    else if (ex is FormatException || ex is IOException || ex is TimeoutException)
                    {
                        Logger.LogError(ex, "Error occured while sending request. {RetryLeft} retries remaining. Request: {Request}.", Retries - attempt + 1, request);
                        if (attempt++ > Retries)
                        {
                            throw;
                        }

                        await DelayRetryWithExceptionHandling(token).ConfigureAwait(false);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            while (!success);

            Logger.LogTrace("Returning modbus response. TransactionId: {TransactionId}, Response: {Response}.", response.TransactionId, response);

            return (TResponse)response;
        }

        public void Dispose()
        {
            if (disposingTokenSource.IsCancellationRequested) // dispose flag
            {
                return;
            }

            lock (disposingTokenSource)
            {
                if (disposingTokenSource.IsCancellationRequested)
                {
                    return;
                }

                disposingTokenSource.Cancel();
                disposingTokenSource.Dispose();

                PipeResource.Dispose();
                semaphoreSlim.Dispose();
            }
        }

        protected abstract Task WriteRequestAsync(IModbusRequest request, CancellationToken token = default);

        protected abstract Task<IModbusResponse> ReadResponseAsync<TResponse>(CancellationToken token = default)
            where TResponse : IModbusResponse, new();

        protected abstract bool RetryReadResponse(IModbusRequest request, IModbusResponse response);

        protected abstract void Validate(IModbusRequest request, IModbusResponse response);

        private void ThrowIfDisposed()
        {
            if (disposingTokenSource.IsCancellationRequested)
            {
                throw new ObjectDisposedException(nameof(ModbusTransport));
            }
        }

        private async Task DelayRetryWithExceptionHandling(CancellationToken token)
        {
            try
            {
                await Task.Delay(WaitToRetryMilliseconds, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                ThrowIfDisposed();
                throw; // else throw OperationCanceledException
            }
        }
    }
}