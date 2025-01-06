using System;
using NModbusAsync.Messages;
using NModbusAsync.Utility;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    public class ModbusResponseFactoryTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void CreatesReadCoilsResponse()
        {
            // Arrange/Act
            ReadCoilsResponse response = (ReadCoilsResponse)ModbusResponseFactory.CreateResponse<ReadCoilsResponse>(
                new byte[] { 11, ModbusFunctionCodes.ReadCoils, 1, 1 });

            // Assert
            Assert.Equal(ModbusFunctionCodes.ReadCoils, response.FunctionCode);
            Assert.Equal(11, response.SlaveAddress);
            Assert.Equal(new bool[] { true, false, false, false }, response.Data.Slice(0, 4).ToArray());
        }

        [Theory]
        [InlineData(new byte[] { 11, ModbusFunctionCodes.ReadCoils })]
        [InlineData(new byte[] { 11, ModbusFunctionCodes.ReadCoils, 4, 1, 2, 3 })]
        [Trait("Category", "Unit")]
        public void CreateReadCoilsResponseThrowsOnFrameTooSmall(byte[] frame)
        {
            // Arrange/Act/Assert
            Assert.Throws<FormatException>(() => ModbusResponseFactory.CreateResponse<ReadCoilsResponse>(frame));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void CreatesReadInputsResponse()
        {
            // Arrange/Act
            ReadInputsResponse response = (ReadInputsResponse)ModbusResponseFactory.CreateResponse<ReadInputsResponse>(
                new byte[] { 11, ModbusFunctionCodes.ReadInputs, 1, 1 });

            // Assert
            Assert.Equal(ModbusFunctionCodes.ReadInputs, response.FunctionCode);
            Assert.Equal(11, response.SlaveAddress);
            Assert.Equal(new bool[] { true, false, false, false }, response.Data.Slice(0, 4).ToArray());
        }

        [Theory]
        [InlineData(new byte[] { 11, ModbusFunctionCodes.ReadInputs })]
        [InlineData(new byte[] { 11, ModbusFunctionCodes.ReadInputs, 4, 1, 2, 3 })]
        [Trait("Category", "Unit")]
        public void CreateReadInputsResponseThrowsOnFrameTooSmall(byte[] frame)
        {
            // Arrange/Act/Assert
            Assert.Throws<FormatException>(() => ModbusResponseFactory.CreateResponse<ReadInputsResponse>(frame));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void CreatesResponseReadHoldingRegistersResponse()
        {
            // Arrange/Act
            ReadHoldingRegistersResponse response = (ReadHoldingRegistersResponse)ModbusResponseFactory
                .CreateResponse<ReadHoldingRegistersResponse>(
                new byte[] { 11, ModbusFunctionCodes.ReadHoldingRegisters, 4, 0, 3, 0, 4 });

            // Assert
            Assert.Equal(ModbusFunctionCodes.ReadHoldingRegisters, response.FunctionCode);
            Assert.Equal(11, response.SlaveAddress);
            Assert.Equal(new ushort[] { 3, 4 }, response.Data);
        }

        [Theory]
        [InlineData(new byte[] { 11, ModbusFunctionCodes.ReadHoldingRegisters })]
        [InlineData(new byte[] { 11, ModbusFunctionCodes.ReadHoldingRegisters, 6, 0, 3, 0, 4 })]
        [Trait("Category", "Unit")]
        public void CreateReadHoldingRegistersResponseThrowsOnFrameTooSmall(byte[] frame)
        {
            // Arrange/Act/Assert
            Assert.Throws<FormatException>(() => ModbusResponseFactory.CreateResponse<ReadHoldingRegistersResponse>(frame));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void CreatesResponseReadInputRegistersResponse()
        {
            // Arrange/Act
            ReadInputRegistersResponse response = (ReadInputRegistersResponse)ModbusResponseFactory
                .CreateResponse<ReadInputRegistersResponse>(
                new byte[] { 11, ModbusFunctionCodes.ReadInputRegisters, 4, 0, 3, 0, 4 });

            // Assert
            Assert.Equal(ModbusFunctionCodes.ReadInputRegisters, response.FunctionCode);
            Assert.Equal(11, response.SlaveAddress);
            Assert.Equal(new ushort[] { 3, 4 }, response.Data);
        }

        [Theory]
        [InlineData(new byte[] { 11, ModbusFunctionCodes.ReadInputRegisters })]
        [InlineData(new byte[] { 11, ModbusFunctionCodes.ReadInputRegisters, 6, 0, 3, 0, 4 })]
        [Trait("Category", "Unit")]
        public void CreateReadInputRegistersResponseThrowsOnFrameTooSmall(byte[] frame)
        {
            // Arrange/Act/Assert
            Assert.Throws<FormatException>(() => ModbusResponseFactory.CreateResponse<ReadInputRegistersResponse>(frame));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void CreatesResponseSlaveExceptionResponse()
        {
            // Arrange/Act
            SlaveExceptionResponse response = (SlaveExceptionResponse)ModbusResponseFactory
                .CreateResponse<SlaveExceptionResponse>(new byte[] { 11, 129, 2 });

            // Assert
            Assert.Equal(129, response.FunctionCode);
            Assert.Equal(11, response.SlaveAddress);
            Assert.Equal(SlaveExceptionCode.IllegalDataAddress, response.SlaveExceptionCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void CreateSlaveExceptionResponseThrowsOnFrameTooSmall()
        {
            // Arrange/Act/Assert
            Assert.Throws<FormatException>(() =>
                ModbusResponseFactory.CreateResponse<SlaveExceptionResponse>(new byte[] { 11, 129 }));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void CreatesWriteMultipleCoilsResponse()
        {
            // Arrange/Act
            WriteMultipleCoilsResponse response = (WriteMultipleCoilsResponse)ModbusResponseFactory
                .CreateResponse<WriteMultipleCoilsResponse>(
                new byte[] { 17, ModbusFunctionCodes.WriteMultipleCoils, 0, 19, 0, 10 });

            // Assert
            Assert.Equal(ModbusFunctionCodes.WriteMultipleCoils, response.FunctionCode);
            Assert.Equal(17, response.SlaveAddress);
            Assert.Equal(19, response.StartAddress);
            Assert.Equal(10, response.NumberOfPoints);
        }

        [Theory]
        [InlineData(new byte[] { 17, ModbusFunctionCodes.WriteMultipleCoils })]
        [InlineData(new byte[] { 17, ModbusFunctionCodes.WriteMultipleCoils, 0, 19, 0 })]
        [Trait("Category", "Unit")]
        public void CreateWriteMultipleCoilsResponseThrowsOnFrameTooSmall(byte[] frame)
        {
            // Arrange/Act/Assert
            Assert.Throws<FormatException>(() =>
                ModbusResponseFactory.CreateResponse<WriteMultipleCoilsResponse>(frame));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void CreatesWriteMultipleRegistersResponse()
        {
            // Arrange/Act
            WriteMultipleRegistersResponse response = (WriteMultipleRegistersResponse)ModbusResponseFactory
                .CreateResponse<WriteMultipleRegistersResponse>(
                new byte[] { 17, ModbusFunctionCodes.WriteMultipleRegisters, 0, 1, 0, 2 });

            // Assert
            Assert.Equal(ModbusFunctionCodes.WriteMultipleRegisters, response.FunctionCode);
            Assert.Equal(17, response.SlaveAddress);
            Assert.Equal(1, response.StartAddress);
            Assert.Equal(2, response.NumberOfPoints);
        }

        [Theory]
        [InlineData(new byte[] { 17, ModbusFunctionCodes.WriteMultipleRegisters })]
        [InlineData(new byte[] { 17, ModbusFunctionCodes.WriteMultipleRegisters, 0, 19, 0 })]
        [Trait("Category", "Unit")]
        public void CreateWriteMultipleRegistersResponseThrowsOnFrameTooSmall(byte[] frame)
        {
            // Arrange/Act/Assert
            Assert.Throws<FormatException>(() =>
                ModbusResponseFactory.CreateResponse<WriteMultipleRegistersResponse>(frame));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void CreatesWriteSingleCoilResponse()
        {
            // Arrange/Act
            WriteSingleCoilResponse response = (WriteSingleCoilResponse)ModbusResponseFactory
                .CreateResponse<WriteSingleCoilResponse>(
                new byte[] { 17, ModbusFunctionCodes.WriteSingleCoil, 0, 172, byte.MaxValue, 0 });

            // Assert
            Assert.Equal(ModbusFunctionCodes.WriteSingleCoil, response.FunctionCode);
            Assert.Equal(17, response.SlaveAddress);
            Assert.Equal(172, response.StartAddress);
            Assert.Equal(Constants.CoilOn, response.Value);
        }

        [Theory]
        [InlineData(new byte[] { 17, ModbusFunctionCodes.WriteSingleCoil })]
        [InlineData(new byte[] { 17, ModbusFunctionCodes.WriteSingleCoil, 0, 105, byte.MaxValue })]
        [Trait("Category", "Unit")]
        public void CreateWriteSingleCoilResponseThrowsOnFrameTooSmall(byte[] frame)
        {
            // Arrange/Act/Assert
            Assert.Throws<FormatException>(() =>
                ModbusResponseFactory.CreateResponse<WriteSingleCoilResponse>(frame));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void CreatesWriteSingleRegisterResponse()
        {
            // Arrange/Act
            WriteSingleRegisterResponse response = (WriteSingleRegisterResponse)ModbusResponseFactory
                .CreateResponse<WriteSingleRegisterResponse>(
                new byte[] { 17, ModbusFunctionCodes.WriteSingleRegister, 0, 1, 0, 3 });

            // Assert
            Assert.Equal(ModbusFunctionCodes.WriteSingleRegister, response.FunctionCode);
            Assert.Equal(17, response.SlaveAddress);
            Assert.Equal(1, response.StartAddress);
            Assert.Equal(3, response.Value);
        }

        [Theory]
        [InlineData(new byte[] { 17, ModbusFunctionCodes.WriteSingleRegister })]
        [InlineData(new byte[] { 17, ModbusFunctionCodes.WriteSingleRegister, 0, 1, 0 })]
        [Trait("Category", "Unit")]
        public void CreateWriteSingleRegisterResponseThrowsOnFrameTooSmall(byte[] frame)
        {
            // Arrange/Act/Assert
            Assert.Throws<FormatException>(() =>
                ModbusResponseFactory.CreateResponse<WriteSingleRegisterResponse>(frame));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadDiscretesResponseDisposesSuccessfullyWhenUninitialized()
        {
            // Arrange
            ReadCoilsResponse response = new ReadCoilsResponse();

            // Act/Assert
            response.Dispose();
        }
    }
}