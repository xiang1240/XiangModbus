using System;

namespace ModbusMaster.ByteUtils
{
    public class ByteConverter
    {
        public static byte[] ToArray(byte[] bytes, int index, int length)
        {
            if (index < 0 || index >= bytes.Length || length < 0 || index + length > bytes.Length)
            {
                throw new ArgumentException();
            }

            byte[] resultBytes = new byte[length];
            Array.Copy(bytes, index, resultBytes, 0, length);

            return resultBytes;
        }

        public static string ToString(byte[] bytes)
        {
            string result = null;

            if (bytes.Length > 0)
            {
                result = "0x" + BitConverter.ToString(bytes, 0).Replace("-", ", 0x");
            }

            return result;
        }
    }
}
