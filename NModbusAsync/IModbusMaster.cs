using System;
using System.Threading;
using System.Threading.Tasks;

namespace NModbusAsync
{
    public interface IModbusMaster : IDisposable
    {
        IModbusTransport Transport { get; }

        Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token = default);

        Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token = default);

        Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token = default);

        Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token = default);

        Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value, CancellationToken token = default);

        Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value, CancellationToken token = default);

        Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data, CancellationToken token = default);

        Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data, CancellationToken token = default);
    }
}