using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NModbusAsync.Data
{
    internal class DiscreteCollection : Collection<bool>, IModbusMessageDataCollection
    {
        private const int BitsPerByte = 8;
        private readonly List<bool> discretes;

        internal DiscreteCollection(params bool[] bits)
            : this((IList<bool>)bits)
        {
        }

        internal DiscreteCollection(params byte[] bytes)
            : this()
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            discretes.Capacity = bytes.Length * BitsPerByte;

            foreach (byte b in bytes)
            {
                discretes.Add((b & 1) == 1);
                discretes.Add((b & 2) == 2);
                discretes.Add((b & 4) == 4);
                discretes.Add((b & 8) == 8);
                discretes.Add((b & 16) == 16);
                discretes.Add((b & 32) == 32);
                discretes.Add((b & 64) == 64);
                discretes.Add((b & 128) == 128);
            }
        }

        private DiscreteCollection()
            : this(new List<bool>())
        {
        }

        private DiscreteCollection(IEnumerable<bool> bits)
            : this(new List<bool>(bits))
        {
        }

        private DiscreteCollection(List<bool> bits)
            : base(bits)
        {
            discretes = bits;
        }

        public byte[] NetworkBytes
        {
            get
            {
                byte[] bytes = new byte[ByteCount];

                for (int index = 0; index < discretes.Count; index++)
                {
                    if (discretes[index])
                    {
                        bytes[index / BitsPerByte] |= (byte)(1 << (index % BitsPerByte));
                    }
                }

                return bytes;
            }
        }

        private byte ByteCount => (byte)((Count + 7) / 8);

        public override string ToString()
        {
            return string.Concat("{", string.Join(", ", this.Select(discrete => discrete ? "1" : "0").ToArray()), "}");
        }
    }
}