using System;
using System.Threading;
using System.Threading.Tasks;
using NModbusAsync.Messages;

namespace NModbusAsync.Devices
{
    internal sealed class ModbusMaster : ModbusDevice, IModbusMaster
    {
        internal ModbusMaster(IModbusTransport transport)
            : base(transport)
        {
        }

        public async Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token = default)
        {
            ValidateNumberOfPoints(numberOfPoints, 2000);

            var request = new ReadCoilsRequest(slaveAddress, startAddress, numberOfPoints);

            using (var response = await Transport.SendAsync<ReadCoilsResponse>(request, token).ConfigureAwait(false))
            {
                return response.Data.Slice(0, request.NumberOfPoints).ToArray();
            }
        }

        public async Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token = default)
        {
            ValidateNumberOfPoints(numberOfPoints, 2000);

            var request = new ReadInputsRequest(slaveAddress, startAddress, numberOfPoints);

            using (var response = await Transport.SendAsync<ReadInputsResponse>(request, token).ConfigureAwait(false))
            {
                return response.Data.Slice(0, request.NumberOfPoints).ToArray();
            }
        }

        public async Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token = default)
        {
            ValidateNumberOfPoints(numberOfPoints, 125);

            var request = new ReadHoldingRegistersRequest(slaveAddress, startAddress, numberOfPoints);

            var response = await Transport.SendAsync<ReadHoldingRegistersResponse>(request, token).ConfigureAwait(false);

            return response.Data;
        }

        public async Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token = default)
        {
            ValidateNumberOfPoints(numberOfPoints, 125);

            var request = new ReadInputRegistersRequest(slaveAddress, startAddress, numberOfPoints);

            var response = await Transport.SendAsync<ReadInputRegistersResponse>(request, token).ConfigureAwait(false);

            return response.Data;
        }

        public Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value, CancellationToken token = default)
        {
            var request = new WriteSingleCoilRequest(slaveAddress, coilAddress, value);
            return Transport.SendAsync<WriteSingleCoilResponse>(request, token);
        }

        public Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value, CancellationToken token = default)
        {
            var request = new WriteSingleRegisterRequest(slaveAddress, registerAddress, value);
            return Transport.SendAsync<WriteSingleRegisterResponse>(request, token);
        }

        public Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data, CancellationToken token = default)
        {
            ValidateData(data, 123);

            var request = new WriteMultipleRegistersRequest(slaveAddress, startAddress, data);
            return Transport.SendAsync<WriteMultipleRegistersResponse>(request, token);
        }

        public Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data, CancellationToken token = default)
        {
            ValidateData(data, 1968);

            var request = new WriteMultipleCoilsRequest(slaveAddress, startAddress, data);
            return Transport.SendAsync<WriteMultipleCoilsResponse>(request, token);
        }

        private static void ValidateData<T>(T[] data, int maxDataLength)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0 || data.Length > maxDataLength)
            {
                throw new ArgumentOutOfRangeException(nameof(data), $"The length must have value between 1 and {maxDataLength} inclusive.");
            }
        }

        private static void ValidateNumberOfPoints(ushort numberOfPoints, ushort maxNumberOfPoints)
        {
            if (numberOfPoints < 1 || numberOfPoints > maxNumberOfPoints)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfPoints), $"Argument must have value between 1 and {maxNumberOfPoints} inclusive.");
            }
        }
    }
}