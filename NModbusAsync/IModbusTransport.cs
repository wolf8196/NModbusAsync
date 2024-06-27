using System;
using System.Threading;
using System.Threading.Tasks;

namespace NModbusAsync
{
    public interface IModbusTransport : IDisposable
    {
        /// <summary>
        /// Gets or sets the number of times to retry sending a request
        /// after encountering a failure such as an IOException, TimeoutException,
        /// or a corrupt message.
        /// </summary>
        uint Retries { get; set; }

        /// <summary>
        /// Gets or sets the threshold for retrying on old transaction IDs.
        /// If set to a non-zero value, it causes a next response to be read
        /// if the current response's transaction ID is behind the transaction ID
        /// of the request by less than this number.
        /// </summary>
        /// <remarks>
        /// For example, setting this property to 3 means that if the response
        /// read when sending request 5 is actually response 3, the system will
        /// attempt to re-read responses to catch up.
        /// Applicable only for TCP transport.
        /// </remarks>
        uint RetryOnOldTransactionIdThreshold { get; set; }

        /// <summary>
        /// Gets or sets the number of times to retry reading a response
        /// if it does not correspond to the request that was sent.
        /// </summary>
        /// <remarks>
        /// Applicable only for RTU over TCP transport.
        /// </remarks>
        uint RetryOnInvalidResponseCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the retry count should be used
        /// when the slave device responds with a 'SlaveDeviceBusy' exception code.
        /// </summary>
        bool SlaveBusyUsesRetryCount { get; set; }

        /// <summary>
        /// Gets or sets the amount of time to wait between retry attempts (in milliseconds).
        /// </summary>
        int WaitToRetryMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the timeout period in milliseconds for reading response.
        /// </summary>
        int ReadTimeout { get; set; }

        /// <summary>
        /// Gets or sets the timeout period in milliseconds for writing request.
        /// </summary>
        int WriteTimeout { get; set; }

        IPipeResource PipeResource { get; }

        Task<TResponse> SendAsync<TResponse>(IModbusRequest request, CancellationToken token = default)
            where TResponse : IModbusResponse, new();
    }
}