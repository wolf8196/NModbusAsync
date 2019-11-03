using System;
using NModbusAsync.Utility;

namespace NModbusAsync.Messages
{
    internal static class ModbusResponseFactory
    {
        internal static IModbusResponse CreateResponse<TResponse>(ReadOnlySpan<byte> frame) where TResponse : IModbusResponse, new()
        {
            byte functionCode = frame[1];
            IModbusResponse response;

            if (functionCode > Constants.ExceptionOffset)
            {
                response = CreateResponseInternal<SlaveExceptionResponse>(frame);
            }
            else
            {
                response = CreateResponseInternal<TResponse>(frame);
            }

            return response;
        }

        private static TResponse CreateResponseInternal<TResponse>(ReadOnlySpan<byte> frame) where TResponse : IModbusResponse, new()
        {
            var message = new TResponse();
            message.Initialize(frame);
            return message;
        }
    }
}