using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using NModbusAsync.Utility;

namespace NModbusAsync.Data
{
    internal class RegisterCollection : Collection<ushort>, IModbusMessageDataCollection
    {
        internal RegisterCollection(byte[] bytes)
            : this((IList<ushort>)ModbusUtility.NetworkBytesToHostUInt16(bytes))
        {
        }

        internal RegisterCollection(params ushort[] registers)
            : this((IList<ushort>)registers)
        {
        }

        private RegisterCollection(IList<ushort> registers)
            : base(registers.IsReadOnly ? new List<ushort>(registers) : registers)
        {
        }

        public byte[] NetworkBytes
        {
            get
            {
                var bytes = new byte[ByteCount];
                var dstOffset = 0;

                foreach (var register in this)
                {
                    var b = BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)register));
                    Buffer.BlockCopy(b, 0, bytes, dstOffset, b.Length);
                    dstOffset += b.Length;
                }

                return bytes;
            }
        }

        internal byte ByteCount => (byte)(Count * 2);

        public override string ToString()
        {
            return string.Concat("{", string.Join(", ", this.Select(v => v.ToString()).ToArray()), "}");
        }
    }
}