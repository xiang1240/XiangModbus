using System;

namespace ModbusMaster.ByteUtils
{
    public static class UInt16Converter
    {
        public static UInt16 FromByteArray(byte[] bytes, bool isLittleEndian = true)
        {
            if (bytes.Length != 2)
            {
                throw new ArgumentException();
            }

            return FromByteArray(bytes, 0, isLittleEndian);
        }

        public static UInt16 FromByteArray(byte[] bytes, int index, bool isLittleEndian = true)
        {
            if (index + 2 > bytes.Length)
            {
                throw new ArgumentException();
            }

            UInt16 value;
            if (isLittleEndian == BitConverter.IsLittleEndian)
            {
                value = (UInt16)(bytes[index] | bytes[index + 1] << 8);
            }
            else
            {
                value = (UInt16)(bytes[index] << 8 | bytes[index + 1]);
            }

            return value;
        }

        public static byte[] ToByteArray(UInt16 value, bool isLittleEndian = true)
        {
            byte[] bytes = new byte[2];

            if (isLittleEndian == BitConverter.IsLittleEndian)
            {
                bytes[0] = (byte)(value & 0xFF);
                bytes[1] = (byte)((value >> 8) & 0xFF);
            }
            else
            {
                bytes[0] = (byte)((value >> 8) & 0xFF);
                bytes[1] = (byte)(value & 0xFF);
            }

            return bytes;
        }

        public static byte[] ToByteArray(UInt16[] values, bool isLittleEndian = true)
        {
            ByteArray byteArray = new ByteArray();
            foreach (UInt16 value in values)
            {
                byteArray.Add(ToByteArray(value, isLittleEndian));
            }

            return byteArray.Array;
        }

        public static UInt16[] ToArray(byte[] bytes, bool isLittleEndian = true)
        {
            int index = 0;
            int length = bytes.Length / 2;

            return ToArray(bytes, index, length, isLittleEndian);
        }

        public static UInt16[] ToArray(byte[] bytes, int index, int length, bool isLittleEndian = true)
        {
            UInt16[] values = new UInt16[length];

            for (int i = 0; i < length; i++)
            {
                values[i] = FromByteArray(bytes, index, isLittleEndian);
                index += 2;
            }

            return values;
        }
    }
}
