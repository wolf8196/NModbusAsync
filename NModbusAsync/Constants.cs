namespace NModbusAsync
{
    internal static class Constants
    {
        internal const string Acknowledge = "Specialized use in conjunction with programming commands.The server (or slave) has accepted the request and is processing it, but a long duration of time will be required to do so.This response is returned to prevent a timeout error from occurring in the client(or master). The client(or master) can next issue a Poll Program Complete message to determine if processing is completed.";
        internal const string EmptyEndPoint = "Argument endPoint cannot be empty.";
        internal const string GatewayPathUnavailable = "Specialized use in conjunction with gateways, indicates that the gateway was unable to allocate an internal communication path from the input port to the output port for processing the request.Usually means that the gateway is misconfigured or overloaded.";
        internal const string GatewayTargetDeviceFailedToRespond = "Specialized use in conjunction with gateways, indicates that no response was obtained from the target device.Usually means that the device is not present on the network.";
        internal const string HexCharacterCountNotEven = "Hex string must have even number of characters.";
        internal const string IllegalDataAddress = "The data address received in the query is not an allowable address for the server (or slave). More specifically, the combination of reference number and transfer length is invalid.For a controller with 100 registers, the PDU addresses the first register as 0, and the last one as 99. If a request is submitted with a starting register address of 96 and a quantity of registers of 4, then this request will successfully operate(address-wise at least) on registers 96, 97, 98, 99. If a request is submitted with a starting register address of 96 and a quantity of registers of 5, then this request will fail with Exception Code 0x02 “Illegal Data Address” since it attempts to operate on registers 96, 97, 98, 99 and 100, and there is no register with address 100.";
        internal const string IllegalDataValue = "A value contained in the query data field is not an allowable value for server(or slave). This indicates a fault in the structure of the remainder of a complex request, such as that the implied length is incorrect.It specifically does NOT mean that a data item submitted for storage in a register has a value outside the expectation of the application program, since the MODBUS protocol is unaware of the significance of any particular value of any particular register.";
        internal const string IllegalFunction = "The function code received in the query is not an allowable action for the server (or slave). This may be because the function code is only applicable to newer devices, and was not implemented in the unit selected.It could also indicate that the server(or slave) is in the wrong state to process a request of this type, for example because it is unconfigured and is being asked to return register values.";
        internal const string MemoryParityError = "Specialized use in conjunction with function codes 20 and 21 and reference type 6, to indicate that the extended file area failed to pass a consistency check.";
        internal const string NetworkBytesNotEven = "Array networkBytes must contain an even number of bytes.";
        internal const string SlaveDeviceBusy = "Specialized use in conjunction with programming commands. The server (or slave) is engaged in processing a long–duration program command.The client(or master) should retransmit the message later when the server(or slave) is free.";
        internal const string SlaveDeviceFailure = "An unrecoverable error occurred while the server(or slave) was attempting to perform the requested action.";
        internal const string SlaveExceptionResponseFormat = "Function Code: {1}{0}Exception Code: {2} - {3}";
        internal const string SlaveExceptionResponseInvalidFunctionCode = "Invalid function code value for SlaveExceptionResponse.";
        internal const string TimeoutNotSupported = "The compact framework UDP client does not support timeouts.";
        internal const string UdpClientNotConnected = "UdpClient must be bound to a default remote host. Call the Connect method.";
        internal const string Unknown = "Unknown slave exception code.";
        internal const string WaitRetryGreaterThanZero = "WaitToRetryMilliseconds must be greater than 0.";
        internal const int MaximumDiscreteRequestResponseSize = 2040;
        internal const int MaximumRegisterRequestResponseSize = 127;
        internal const byte ExceptionOffset = 128;
        internal const int DefaultRetries = 3;
        internal const int DefaultWaitToRetryMilliseconds = 250;
        internal const ushort CoilOn = 0xFF00;
        internal const ushort CoilOff = 0x0000;
        internal const byte DefaultIpSlaveUnitId = 0;
        internal const string NewLine = "\r\n";
    }
}