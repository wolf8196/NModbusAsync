using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbusAsync.Message;
using NModbusAsync.Utility;

namespace NModbusAsync.IO
{
    internal abstract class ModbusTransport : IModbusTransport
    {
        private SemaphoreSlim semaphoreSlim;
        private int waitToRetryMilliseconds;
        private IStreamResource streamResource;

        internal ModbusTransport(IStreamResource streamResource, IModbusLogger logger)
        {
            this.streamResource = streamResource ?? throw new ArgumentNullException(nameof(streamResource));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            semaphoreSlim = new SemaphoreSlim(1, 1);
            waitToRetryMilliseconds = Constants.DefaultWaitToRetryMilliseconds;
            Retries = Constants.DefaultRetries;
        }

        public int Retries { get; set; }

        public uint RetryOnOldResponseThreshold { get; set; }

        public bool SlaveBusyUsesRetryCount { get; set; }

        public int WaitToRetryMilliseconds
        {
            get => waitToRetryMilliseconds;

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(Constants.WaitRetryGreaterThanZero);
                }

                waitToRetryMilliseconds = value;
            }
        }

        public int ReadTimeout
        {
            get => StreamResource.ReadTimeout;
            set => StreamResource.ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => StreamResource.WriteTimeout;
            set => StreamResource.WriteTimeout = value;
        }

        public IStreamResource StreamResource => streamResource;

        protected IModbusLogger Logger { get; }

        public void Dispose()
        {
            DisposableUtility.Dispose(ref semaphoreSlim);
            DisposableUtility.Dispose(ref streamResource);
        }

        public virtual async Task<T> UnicastMessageAsync<T>(IModbusMessage message, CancellationToken token) where T : IModbusMessage, new()
        {
            IModbusMessage response = null;
            int attempt = 1;
            bool success = false;

            do
            {
                try
                {
                    await semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
                    try
                    {
                        await WriteAsync(message, token).ConfigureAwait(false);

                        bool readAgain;
                        do
                        {
                            readAgain = false;
                            response = await ReadResponseAsync<T>(token).ConfigureAwait(false);

                            if (response is SlaveExceptionResponse exceptionResponse)
                            {
                                // if SlaveExceptionCode == ACKNOWLEDGE we retry reading the response without resubmitting request
                                readAgain = exceptionResponse.SlaveExceptionCode == SlaveExceptionCodes.Acknowledge;

                                if (readAgain)
                                {
                                    Logger.Log(LoggingLevel.Debug, $"Received ACKNOWLEDGE slave exception response, waiting {waitToRetryMilliseconds} milliseconds and retrying to read response.");
                                    await Task.Delay(WaitToRetryMilliseconds, token).ConfigureAwait(false);
                                }
                                else
                                {
                                    throw new SlaveException(exceptionResponse);
                                }
                            }
                            else if (ShouldRetryResponse(message, response))
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

                    ValidateResponse(message, response);
                    success = true;
                }
                catch (SlaveException se)
                {
                    if (se.SlaveExceptionCode != SlaveExceptionCodes.SlaveDeviceBusy
                        || (SlaveBusyUsesRetryCount && attempt++ > Retries))
                    {
                        throw;
                    }

                    Logger.Log(LoggingLevel.Warning, $"Received SLAVE_DEVICE_BUSY exception response, waiting {waitToRetryMilliseconds} milliseconds and resubmitting request.");
                    await Task.Delay(WaitToRetryMilliseconds, token).ConfigureAwait(false);
                }
                catch (Exception e) when (e is SocketException || e.InnerException is SocketException)
                {
                    throw;
                }
                catch (Exception e) when (e is FormatException || e is NotImplementedException || e is TimeoutException || e is IOException)
                {
                    Logger.Log(LoggingLevel.Error, $"{e.GetType().Name}, {(Retries - attempt + 1)} retries remaining - {e}");

                    if (attempt++ > Retries)
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            while (!success);

            return (T)response;
        }

        public abstract Task<byte[]> ReadRequestAsync(CancellationToken token);

        public abstract byte[] BuildMessageFrame(IModbusMessage message);

        public abstract Task WriteAsync(IModbusMessage message, CancellationToken token);

        protected virtual IModbusMessage CreateResponse<T>(byte[] frame) where T : IModbusMessage, new()
        {
            byte functionCode = frame[1];
            IModbusMessage response;

            // check for slave exception response else create message from frame
            if (functionCode > Constants.ExceptionOffset)
            {
                response = ModbusMessageFactory.CreateModbusMessage<SlaveExceptionResponse>(frame);
            }
            else
            {
                response = ModbusMessageFactory.CreateModbusMessage<T>(frame);
            }

            return response;
        }

        protected virtual bool OnShouldRetryResponse(IModbusMessage request, IModbusMessage response)
        {
            return false;
        }

        protected abstract Task<IModbusMessage> ReadResponseAsync<T>() where T : IModbusMessage, new();

        protected abstract Task<IModbusMessage> ReadResponseAsync<T>(CancellationToken token) where T : IModbusMessage, new();

        protected abstract void OnValidateResponse(IModbusMessage request, IModbusMessage response);

        private void ValidateResponse(IModbusMessage request, IModbusMessage response)
        {
            // always check the function code and slave address, regardless of transport protocol
            if (request.FunctionCode != response.FunctionCode)
            {
                string msg = $"Received response with unexpected Function Code. Expected {request.FunctionCode}, received {response.FunctionCode}.";
                throw new IOException(msg);
            }

            if (request.SlaveAddress != response.SlaveAddress)
            {
                string msg = $"Response slave address does not match request. Expected {response.SlaveAddress}, received {request.SlaveAddress}.";
                throw new IOException(msg);
            }

            // message specific validation
            if (request is IModbusRequest req)
            {
                req.ValidateResponse(response);
            }

            OnValidateResponse(request, response);
        }

        private bool ShouldRetryResponse(IModbusMessage request, IModbusMessage response)
        {
            // These checks are enforced in ValidateRequest, we don't want to retry for these
            if (request.FunctionCode != response.FunctionCode)
            {
                return false;
            }

            if (request.SlaveAddress != response.SlaveAddress)
            {
                return false;
            }

            return OnShouldRetryResponse(request, response);
        }
    }
}