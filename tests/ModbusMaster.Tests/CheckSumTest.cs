using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModbusMaster.Protocal;
using System;
using System.Text;

namespace ModbusMaster.Tests
{
    [TestClass]
    public class CheckSumTest
    {
        /// <summary>
        /// Read 12 coils starting at 00033 from a PLC at address 2. 
        /// 02 01 00 20 00 0C => 3D F6
        /// </summary>
        [TestMethod]
        public void CalculateCrc()
        {
            byte[] bytes = new byte[6] { 0x02, 0x01, 0x00, 0x20, 0x00, 0x0C };
            byte[] result = BitConverter.GetBytes(CheckSum.CalculateCrc(bytes));

            Assert.AreEqual(result[0], 0x3D);
            Assert.AreEqual(result[1], 0xF6);
        }

        /// <summary>
        /// Read 12 coils starting at 00033 from a PLC at address 2. 
        /// 02 01 00 20 00 0C => 44 31
        /// </summary>
        [TestMethod]
        public void CalculateLrc()
        {
            byte[] bytes = new byte[6] { 0x02, 0x01, 0x00, 0x20, 0x00, 0x0C };

            byte lrc = CheckSum.CalculateLrc(bytes);
            string text = lrc.ToString("X2");
            byte[] result = Encoding.ASCII.GetBytes(text);
            Assert.AreEqual(result[0], 0x44);
            Assert.AreEqual(result[1], 0x31);
        }
    }
}
