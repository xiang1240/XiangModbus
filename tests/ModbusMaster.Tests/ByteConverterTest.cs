using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ModbusMaster.ByteUtils;
using System;

namespace ModbusMaster.Tests
{
    [TestClass]
    public class ByteConverterTest
    {
        [TestMethod]
        public void GetHexValueTest()
        {
            byte[] charBytes = new byte[] { 0x41, 0x44 };
            byte[] bytes = HexConverter.ToBytesFromHexCharBytes(charBytes);

            Assert.AreEqual(bytes[0], 0xad);
        }

        [TestMethod]
        public void IsLittleEndianTest()
        {
            Assert.IsTrue(BitConverter.IsLittleEndian);
        }

        [TestMethod]
        public void ToBytesTest()
        {
            ushort value = 0x1234;
            byte[] result;

            result = UInt16Converter.ToByteArray(value, false);
            Assert.IsTrue(Enumerable.SequenceEqual(result, new byte[] { 0x12, 0x34 }));

            result = UInt16Converter.ToByteArray(value);
            Assert.IsTrue(Enumerable.SequenceEqual(result, new byte[] { 0x34, 0x12 }));
        }

        [TestMethod()]
        public void ToStringTest()
        {
            string expectResult = "0x02, 0x01, 0x00, 0x20, 0x07, 0xD0, 0x3E, 0x5F";
            byte[] responce = new byte[] { 0x02, 0x01, 0x00, 0x20, 0x07, 0xD0, 0x3E, 0x5F };

            string result = ByteConverter.ToString(responce);
            Assert.AreEqual(result, expectResult);
        }
    }
}
