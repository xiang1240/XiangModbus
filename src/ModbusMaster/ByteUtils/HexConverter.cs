using System;

namespace ModbusMaster.ByteUtils
{
    public static class HexConverter
    {
        public static char[] ToHexChars(byte[] bytes)
        {
            int length = bytes.Length;
            char[] chars = new char[length * 2];
            for (int i = 0, j = 0; i < length; i++, j += 2)
            {
                byte b = bytes[i];

                chars[j] = GetHexChar(b / 16);
                chars[j + 1] = GetHexChar(b % 16);
            }

            return chars;
        }

        public static char[] ToHexChars(byte b)
        {
            char[] chars = new char[2];
            chars[0] = GetHexChar(b / 16);
            chars[1] = GetHexChar(b % 16);

            return chars;
        }

        public static byte[] ToBytesFromHexCharBytes(byte[] charBytes)
        {
            int length = charBytes.Length;
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = (char)charBytes[i];
            }

            return ToBytesFromHexChars(chars);
        }

        public static byte[] ToBytesFromHexChars(char[] chars)
        {
            int length = chars.Length / 2;
            byte[] bytes = new byte[length];
            for (int i = 0, j = 0; i < length; i++, j += 2)
            {
                bytes[i] = GetHexValue(chars, j);
            }

            return bytes;
        }

        private static char GetHexChar(int i)
        {
            if (i < 10)
            {
                return (char)(i + '0');
            }

            return (char)(i - 10 + 'A');
        }

        private static byte GetHexValue(char[] chars, int start)
        {
            if (start < 0 || start > chars.Length - 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            return (byte)(GetHexValue(chars[start]) << 4 | (GetHexValue(chars[start + 1]) & 0xF));
        }

        private static byte GetHexValue(char c)
        {
            c = char.ToUpper(c);
            if (c >= 'A')
            {
                return (byte)(c - 'A' + 10);
            }

            return (byte)(c - '0');
        }
    }
}
