using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NModbusAsync.Data;

namespace NModbusAsync.Message
{
    internal class ModbusMessageImpl
    {
        // smallest supported message frame size (sans checksum)
        private const int MinimumFrameSize = 2;

        internal ModbusMessageImpl()
        {
        }

        internal ModbusMessageImpl(byte slaveAddress, byte functionCode)
        {
            SlaveAddress = slaveAddress;
            FunctionCode = functionCode;
        }

        internal byte? ByteCount { get; set; }

        internal byte? ExceptionCode { get; set; }

        internal ushort TransactionId { get; set; }

        internal byte FunctionCode { get; set; }

        internal ushort? NumberOfPoints { get; set; }

        internal byte SlaveAddress { get; set; }

        internal ushort? StartAddress { get; set; }

        internal IModbusMessageDataCollection Data { get; set; }

        internal byte[] MessageFrame
        {
            get
            {
                var pdu = ProtocolDataUnit;
                var frame = new byte[1 + pdu.Length];
                frame[0] = SlaveAddress;
                Buffer.BlockCopy(pdu, 0, frame, 1, pdu.Length);
                return frame;
            }
        }

        internal byte[] ProtocolDataUnit
        {
            get
            {
                var pdu = new List<byte> { FunctionCode };

                if (ExceptionCode.HasValue)
                {
                    pdu.Add(ExceptionCode.Value);
                }

                if (StartAddress.HasValue)
                {
                    pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)StartAddress.Value)));
                }

                if (NumberOfPoints.HasValue)
                {
                    pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)NumberOfPoints.Value)));
                }

                if (ByteCount.HasValue)
                {
                    pdu.Add(ByteCount.Value);
                }

                if (Data != null)
                {
                    pdu.AddRange(Data.NetworkBytes);
                }

                return pdu.ToArray();
            }
        }

        internal void Initialize(byte[] frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame), "Argument frame cannot be null.");
            }

            if (frame.Length < MinimumFrameSize)
            {
                string msg = $"Message frame must contain at least {MinimumFrameSize} bytes of data.";
                throw new FormatException(msg);
            }

            SlaveAddress = frame[0];
            FunctionCode = frame[1];
        }
    }
}