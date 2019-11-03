using System;
using System.Diagnostics.CodeAnalysis;
using NModbusAsync.Messages;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    [ExcludeFromCodeCoverage]
    public class SlaveExceptionTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ThrowsOnInvalidFunctionCode()
        {
            // Arrange
            var response = new SlaveExceptionResponse();

            // Act/Assert
            Assert.Throws<FormatException>(() => response.Initialize(new byte[] { 1, 10, 1 }));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void CreatesSlaveExceptionFromSlaveExceptionResponse()
        {
            // Arrange
            var response = new SlaveExceptionResponse();
            response.Initialize(new byte[] { 1, 129, 2 });

            // Act
            var exception = new SlaveException(response);

            // Assert
            Assert.Equal(1, exception.SlaveAddress);
            Assert.Equal(129, exception.FunctionCode);
            Assert.Equal(SlaveExceptionCode.IllegalDataAddress, exception.SlaveExceptionCode);
            Assert.Contains(response.ToString(), exception.Message);
        }
    }
}