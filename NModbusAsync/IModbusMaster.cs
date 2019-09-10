using System;
using System.Threading;
using System.Threading.Tasks;

namespace NModbusAsync
{
    public interface IModbusMaster : IDisposable
    {
        IModbusTransport Transport { get; }

        Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints);

        Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token);

        Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints);

        Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token);

        Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints);

        Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token);

        Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints);

        Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, CancellationToken token);

        Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value);

        Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value, CancellationToken token);

        Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value);

        Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value, CancellationToken token);

        Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data);

        Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data, CancellationToken token);

        Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data);

        Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data, CancellationToken token);
    }
}