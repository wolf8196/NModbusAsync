using System.Diagnostics.CodeAnalysis;
using NModbusAsync.Messages;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    [ExcludeFromCodeCoverage]
    public class MessageToStringTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ReadCoilsRequestToString()
        {
            // Arrange
            var request = new ReadCoilsRequest(5, 1, 10);

            // Act/Assert
            Assert.Equal("Read 10 coils starting at address 1 from slave 5.", request.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadInputsRequestToString()
        {
            // Arrange
            var request = new ReadInputsRequest(5, 1, 10);

            // Act/Assert
            Assert.Equal("Read 10 inputs starting at address 1 from slave 5.", request.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadHoldingRegistersRequestToString()
        {
            // Arrange
            var request = new ReadHoldingRegistersRequest(5, 1, 10);

            // Act/Assert
            Assert.Equal("Read 10 holding registers starting at address 1 from slave 5.", request.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadInputRegistersRequestToString()
        {
            // Arrange
            var request = new ReadInputRegistersRequest(5, 1, 10);

            // Act/Assert
            Assert.Equal("Read 10 input registers starting at address 1 from slave 5.", request.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteMultipleCoilsRequestToString()
        {
            // Arrange
            var request = new WriteMultipleCoilsRequest(34, 45, new bool[] { true, false, true, false, true, true, true, false, false });

            // Act/Assert
            Assert.Equal("Write 9 coils starting at address 45 into slave 34.", request.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteMultipleRegistersRequestToString()
        {
            // Arrange
            var request = new WriteMultipleRegistersRequest(11, 34, new ushort[] { 10, 20, 30, 40, 50 });

            // Act/Assert
            Assert.Equal("Write 5 holding registers starting at address 34 into slave 11.", request.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteSingleCoilRequestToStringWithTrue()
        {
            // Arrange
            var request = new WriteSingleCoilRequest(11, 5, true);

            // Act/Assert
            Assert.Equal("Write single coil 1 at address 5 into slave 11.", request.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteSingleCoilRequestToStringWithFalse()
        {
            // Arrange
            var request = new WriteSingleCoilRequest(11, 5, false);

            // Act/Assert
            Assert.Equal("Write single coil 0 at address 5 into slave 11.", request.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadCoilsResponseToString()
        {
            // Arrange
            var response = new ReadCoilsResponse();
            response.Initialize(new byte[] { 5, ModbusFunctionCodes.ReadCoils, 2, 1, 1 });

            // Act/Assert
            Assert.Equal($"Read 16 coils from slave 5.", response.ToString());
            response.Dispose();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadInputsResponseToString()
        {
            // Arrange
            var response = new ReadInputsResponse();
            response.Initialize(new byte[] { 5, ModbusFunctionCodes.ReadInputs, 2, 1, 1 });

            // Act/Assert
            Assert.Equal($"Read 16 inputs from slave 5.", response.ToString());
            response.Dispose();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadHoldingRegistersResponseToString()
        {
            // Arrange
            var response = new ReadHoldingRegistersResponse();
            response.Initialize(new byte[] { 1, ModbusFunctionCodes.ReadHoldingRegisters, 4, 0, 1, 0, 1 });

            // Act/Assert
            Assert.Equal("Read 2 holding registers from slave 1.", response.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadInputRegistersResponseToString()
        {
            // Arrange
            var response = new ReadInputRegistersResponse();
            response.Initialize(new byte[] { 1, ModbusFunctionCodes.ReadInputRegisters, 4, 0, 1, 0, 1 });

            // Act/Assert
            Assert.Equal("Read 2 input registers from slave 1.", response.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteMultipleCoilsResponseToString()
        {
            // Arrange
            var response = new WriteMultipleCoilsResponse();
            response.Initialize(new byte[] { 17, ModbusFunctionCodes.WriteMultipleCoils, 0, 19, 0, 10 });

            // Act/Assert
            Assert.Equal("Wrote 10 coils starting at address 19 into slave 17.", response.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteMultipleRegistersResponseToString()
        {
            // Arrange
            var response = new WriteMultipleRegistersResponse();
            response.Initialize(new byte[] { 17, ModbusFunctionCodes.WriteMultipleRegisters, 0, 1, 0, 2 });

            // Act/Assert
            Assert.Equal("Wrote 2 holding registers starting at address 1 into slave 17.", response.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteSingleCoilResponseToStringWithTrue()
        {
            // Arrange
            var response = new WriteSingleCoilResponse();
            response.Initialize(new byte[] { 17, ModbusFunctionCodes.WriteSingleCoil, 0, 172, byte.MaxValue, 0 });

            // Act/Assert
            Assert.Equal("Wrote single coil 1 at address 172 into slave 17.", response.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteSingleCoilResponseToStringWithFalse()
        {
            // Arrange
            var response = new WriteSingleCoilResponse();
            response.Initialize(new byte[] { 17, ModbusFunctionCodes.WriteSingleCoil, 0, 172, 0, 0 });

            // Act/Assert
            Assert.Equal("Wrote single coil 0 at address 172 into slave 17.", response.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void SlaveExceptionResponseToStringWithUnknownExceptionCode()
        {
            // Arrange
            var response = new SlaveExceptionResponse();
            response.Initialize(new byte[] { 11, 129, 20 });

            // Act/Assert
            Assert.Equal(
                $@"Function Code: 129.
Exception Code: 20.
Message: Unknown slave exception code.
Slave: 11.",
response.ToString());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void SlaveExceptionResponseToStringWithKnownExceptionCode()
        {
            // Arrange
            var response = new SlaveExceptionResponse();
            response.Initialize(new byte[] { 11, 129, 1 });

            // Act/Assert
            Assert.Equal(
                $@"Function Code: 129.
Exception Code: 1.
Message: The function code received in the query is not an allowable action for the server (or slave). This may be because the function code is only applicable to newer devices, and was not implemented in the unit selected.It could also indicate that the server(or slave) is in the wrong state to process a request of this type, for example because it is unconfigured and is being asked to return register values.
Slave: 11.",
response.ToString());
        }
    }
}