using System;

namespace ModbusMaster.ByteUtils
{
    public static class BoolConverter
    {
        public static bool GetValue(byte value, int bit)
        {
            return (((int)value & (1 << bit)) != 0);
        }

        public static byte SetBit(byte value, int bit)
        {
            SetBit(ref value, bit);
            return value;
        }

        public static void SetBit(ref byte value, int bit)
        {
            value = (byte)((value | (1 << bit)) & 0xFF);
        }

        public static byte ClearBit(byte value, int bit)
        {
            ClearBit(ref value, bit);
            return value;
        }

        public static void ClearBit(ref byte value, int bit)
        {
            value = (byte)(value & ~(1 << bit) & 0xFF);
        }

        public static byte[] ToByteArray(bool[] values)
        {
            byte[] bytes = new byte[(values.Length + 7) / 8];
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i])
                {
                    SetBit(ref bytes[i / 8], i % 8);
                }
            }

            return bytes;
        }

        public static bool[] ToArray(byte[] bytes)
        {
            int index = 0;
            int length = bytes.Length * 8;

            return ToArray(bytes, index, length);
        }

        public static bool[] ToArray(byte[] bytes, int index, int length)
        {
            if (bytes.Length - index < (length + 7) / 8)
            {
                throw new ArgumentException();
            }

            bool[] values = new bool[length];

            for (int i = 0; i < length; i++)
            {
                values[i] = GetValue(bytes[index + i / 8], i % 8);
            }

            return values;
        }
    }
}
