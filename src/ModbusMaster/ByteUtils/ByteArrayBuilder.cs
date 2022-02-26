using System;

namespace ModbusMaster.ByteUtils
{
    public class ByteArrayBuilder
    {
        private ByteSegementCollection _segements;

        public ByteSegementCollection Segements
        {
            get
            {
                if (_segements == null)
                {
                    _segements = new ByteSegementCollection();
                }

                return _segements;
            }
        }

        public ByteSegement Append(byte value, string name = null)
        {
            ByteSegement segement = new ByteSegement(name);
            segement.Value = value;
            Segements.Add(segement);

            return segement;
        }

        public UInt16Segement Append(ushort value, bool isLittleEndian = true, string name = null)
        {
            UInt16Segement segement = new UInt16Segement(isLittleEndian, name);
            segement.Value = value;
            Segements.Add(segement);

            return segement;
        }

        public ByteArraySegement Append(byte[] value, string name = null)
        {
            ByteArraySegement segement = new ByteArraySegement(name);
            segement.Value = value;
            Segements.Add(segement);

            return segement;
        }

        public UInt16ArraySegement Append(ushort[] value, string name = null)
        {
            UInt16ArraySegement segement = new UInt16ArraySegement(name);
            segement.Value = value;
            Segements.Add(segement);

            return segement;
        }

        public ByteSegementBase GetSegement(string name)
        {
            return Segements[name];
        }

        public int Length
        {
            get
            {
                return Segements.Length;
            }
        }

        public Byte[] ToByteArray()
        {
            return Segements.ToByteArray();
        }

        public override string ToString()
        {
            return Segements.ToString();
        }
    }
}
