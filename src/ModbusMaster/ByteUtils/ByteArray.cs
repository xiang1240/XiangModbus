using System.Collections.Generic;

namespace ModbusMaster.ByteUtils
{
    public class ByteArray
    {
        List<byte> bytes = new List<byte>();

        public byte this[int index]
        {
            get => bytes[index];
            set => bytes[index] = value;
        }

        public byte[] Array
        {
            get { return bytes.ToArray(); }
        }

        public int Length => bytes.Count;

        public ByteArray()
        {
            bytes = new List<byte>();
        }

        public ByteArray(int size)
        {
            bytes = new List<byte>(size);
        }

        public void Clear()
        {
            bytes = new List<byte>();
        }

        public void Add(byte item)
        {
            bytes.Add(item);
        }

        public void AddWord(ushort value)
        {
            bytes.Add((byte)(value >> 8));
            bytes.Add((byte)value);
        }

        public void Add(byte[] items)
        {
            bytes.AddRange(items);
        }

        public void Add(IEnumerable<byte> items)
        {
            bytes.AddRange(items);
        }

        public void Add(ByteArray byteArray)
        {
            bytes.AddRange(byteArray.Array);
        }
    }
}
