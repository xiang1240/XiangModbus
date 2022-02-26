using System;
using System.Text;

namespace ModbusMaster.ByteUtils
{
    public abstract class ByteSegementBase
    {
        private static bool _systemIsLittleEndian = BitConverter.IsLittleEndian;

        public static bool SystemIsLittleEndian
        {
            get
            {
                return _systemIsLittleEndian;
            }
        }

        public ByteSegementBase(string name = null)
        {
            Name = name;
        }

        public ByteSegementBase(bool isLittleEndian = true, string name = null)
        {
            IsLittleEndian = isLittleEndian;
            Name = name;
        }

        public ByteSegementCollection Segements { get; set; }

        public bool IsLittleEndian { get; private set; } = true;

        public string Name { get; private set; }

        public abstract int Length { get; }

        public int Offset
        {
            get
            {
                return Segements.GetSegementOffset(this);
            }
        }

        public abstract byte[] ToByteArray();
    }

    public class ByteSegement : ByteSegementBase
    {
        public ByteSegement(string name = null) : base(name)
        {
        }

        public byte Value { get; set; }

        public override int Length
        {
            get
            {
                return sizeof(byte);
            }
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { Value };
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return string.Format("{0}: {1:X}", Name, Value);
            }

            return Value.ToString("X");
        }
    }

    public class UInt16Segement : ByteSegementBase
    {
        public UInt16Segement(bool isLittleEndian = true, string name = null) : base(isLittleEndian, name)
        {
        }

        public UInt16 Value { get; set; }

        public override int Length
        {
            get
            {
                return sizeof(ushort);
            }
        }

        public override byte[] ToByteArray()
        {
            byte[] result = new byte[2];

            if (IsLittleEndian == SystemIsLittleEndian)
            {
                result[0] = (byte)Value;
                result[1] = (byte)(Value >> 8);
            }
            else
            {
                result[0] = (byte)(Value >> 8);
                result[1] = (byte)Value;
            }

            return result;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return string.Format("{0}: {1:X}", Name, Value);
            }

            return Value.ToString("X");
        }
    }

    public class ByteArraySegement : ByteSegementBase
    {
        public ByteArraySegement(string name = null) : base(name)
        {
        }

        public byte[] Value { get; set; }

        public override int Length
        {
            get
            {
                return sizeof(byte) * Value.Length;
            }
        }

        public override byte[] ToByteArray()
        {
            return Value;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(Name))
            {
                stringBuilder.Append(string.Format("{0}:", Name));
            }

            foreach (byte b in Value)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(" ");
                }
                stringBuilder.Append(b.ToString("X"));
            }

            return stringBuilder.ToString();
        }
    }

    public class UInt16ArraySegement : ByteSegementBase
    {
        public UInt16ArraySegement(string name = null) : base(name)
        {
        }

        public UInt16[] Value { get; set; }

        public override int Length
        {
            get
            {
                return sizeof(UInt16) * Value.Length;
            }
        }

        public override byte[] ToByteArray()
        {
            int length = Value.Length;
            byte[] result = new byte[length * 2];

            if (IsLittleEndian == SystemIsLittleEndian)
            {
                for (int i = 0, j = 0; i < length; i++, j += 2)
                {
                    result[j] = (byte)Value[i];
                    result[j + 1] = (byte)(Value[i] >> 8);
                }
            }
            else
            {
                for (int i = 0, j = 0; i < length; i++, j += 2)
                {
                    result[j] = (byte)(Value[i] >> 8);
                    result[j + 1] = (byte)Value[i];
                }
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(Name))
            {
                stringBuilder.Append(string.Format("{0}:", Name));
            }

            foreach (ushort us in Value)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(" ");
                }
                stringBuilder.Append(us.ToString("X"));
            }

            return stringBuilder.ToString();
        }
    }
}
