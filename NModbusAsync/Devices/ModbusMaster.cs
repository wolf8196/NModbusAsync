using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NModbusAsync.Data;
using NModbusAsync.Message;

namespace NModbusAsync.Devices
{
    internal abstract class ModbusMaster : ModbusDevice, IModbusMaster
    {
        internal ModbusMaster(IModbusTransport transport)
            : base(transport)
        {
        }

        public Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            return ReadCoilsAsync(slaveAddress, startAddress, numberOfPoints, default);
        }

        public Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token)
        {
            ValidateNumberOfPoints(numberOfPoints, 2000);

            var request = new ReadCoilsInputsRequest(
                ModbusFunctionCodes.ReadCoils,
                slaveAddress,
                startAddress,
                numberOfPoints);

            return PerformReadDiscretesAsync(request, token);
        }

        public Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            return ReadInputsAsync(slaveAddress, startAddress, numberOfPoints, default);
        }

        public Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token)
        {
            ValidateNumberOfPoints(numberOfPoints, 2000);

            var request = new ReadCoilsInputsRequest(
                ModbusFunctionCodes.ReadInputs,
                slaveAddress,
                startAddress,
                numberOfPoints);

            return PerformReadDiscretesAsync(request, token);
        }

        public Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            return ReadHoldingRegistersAsync(slaveAddress, startAddress, numberOfPoints, default);
        }

        public Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token)
        {
            ValidateNumberOfPoints(numberOfPoints, 125);

            var request = new ReadHoldingInputRegistersRequest(
                ModbusFunctionCodes.ReadHoldingRegisters,
                slaveAddress,
                startAddress,
                numberOfPoints);

            return PerformReadRegistersAsync(request, token);
        }

        public Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            return ReadInputRegistersAsync(slaveAddress, startAddress, numberOfPoints, default);
        }

        public Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token)
        {
            ValidateNumberOfPoints(numberOfPoints, 125);

            var request = new ReadHoldingInputRegistersRequest(
                ModbusFunctionCodes.ReadInputRegisters,
                slaveAddress,
                startAddress,
                numberOfPoints);

            return PerformReadRegistersAsync(request, token);
        }

        public Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value)
        {
            return WriteSingleCoilAsync(slaveAddress, coilAddress, value, default);
        }

        public Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value, CancellationToken token)
        {
            var request = new WriteSingleCoilRequestResponse(slaveAddress, coilAddress, value);
            return Transport.UnicastMessageAsync<WriteSingleCoilRequestResponse>(request, token);
        }

        public Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value)
        {
            return WriteSingleRegisterAsync(slaveAddress, registerAddress, value, default);
        }

        public Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value, CancellationToken token)
        {
            var request = new WriteSingleRegisterRequestResponse(
                slaveAddress,
                registerAddress,
                value);

            return Transport.UnicastMessageAsync<WriteSingleRegisterRequestResponse>(request, token);
        }

        public Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data)
        {
            return WriteMultipleRegistersAsync(slaveAddress, startAddress, data, default);
        }

        public Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data, CancellationToken token)
        {
            ValidateData("data", data, 123);

            var request = new WriteMultipleRegistersRequest(
                slaveAddress,
                startAddress,
                new RegisterCollection(data));

            return Transport.UnicastMessageAsync<WriteMultipleRegistersResponse>(request, token);
        }

        public Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data)
        {
            return WriteMultipleCoilsAsync(slaveAddress, startAddress, data, default);
        }

        public Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data, CancellationToken token)
        {
            ValidateData("data", data, 1968);

            var request = new WriteMultipleCoilsRequest(
                slaveAddress,
                startAddress,
                new DiscreteCollection(data));

            return Transport.UnicastMessageAsync<WriteMultipleCoilsResponse>(request, token);
        }

        private static void ValidateData<T>(string argumentName, T[] data, int maxDataLength)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0 || data.Length > maxDataLength)
            {
                throw new ArgumentException($"The length of argument {argumentName} must be between 1 and {maxDataLength} inclusive.");
            }
        }

        private static void ValidateNumberOfPoints(ushort numberOfPoints, ushort maxNumberOfPoints)
        {
            if (numberOfPoints < 1 || numberOfPoints > maxNumberOfPoints)
            {
                throw new ArgumentException($"Argument {nameof(numberOfPoints)} must be between 1 and {maxNumberOfPoints} inclusive.");
            }
        }

        private async Task<bool[]> PerformReadDiscretesAsync(ReadCoilsInputsRequest request, CancellationToken token)
        {
            ReadCoilsInputsResponse response = await Transport
                .UnicastMessageAsync<ReadCoilsInputsResponse>(request, token)
                .ConfigureAwait(false);
            return response.Data.Take(request.NumberOfPoints).ToArray();
        }

        private async Task<ushort[]> PerformReadRegistersAsync(ReadHoldingInputRegistersRequest request, CancellationToken token)
        {
            ReadHoldingInputRegistersResponse response = await Transport
                .UnicastMessageAsync<ReadHoldingInputRegistersResponse>(request, token)
                .ConfigureAwait(false);

            return response.Data.Take(request.NumberOfPoints).ToArray();
        }
    }
}