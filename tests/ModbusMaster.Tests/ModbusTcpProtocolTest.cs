using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModbusMaster.ByteUtils;
using ModbusMaster.Protocal;
using System;
using System.Linq;

namespace ModbusMaster.Tests
{
    [TestClass]
    public class ModbusTcpProtocolTest
    {
        /// <summary>
        /// Example: Read 12 coils starting at 00033 from a PLC at address 2. Response with coils 00040 and 00042 set and all others clear :
        /// Request: 00 05 00 00 00 06 02 01 00 20 00 0C
        /// </summary>
        [TestMethod]
        public void BuildReadCoilsRequest()
        {
            ModbusTcpProtocol.Current.TransactionId = 5;
            byte[] result = ModbusTcpProtocol.Current.BuildReadCoilsRequest(2, 32, 12);

            Assert.AreEqual(result.Length, 12);

            // Transaction Id
            Assert.AreEqual(result[0], 0x00);
            Assert.AreEqual(result[1], 0x05);

            // Protocol
            Assert.AreEqual(result[2], 0x00);
            Assert.AreEqual(result[3], 0x00);

            // Length
            Assert.AreEqual(result[4], 0x00);
            Assert.AreEqual(result[5], 0x06);

            // Slave Address
            Assert.AreEqual(result[6], 0x02);

            // Function Code
            Assert.AreEqual(result[7], 0x01);

            // First coil address
            Assert.AreEqual(result[8], 0x00);
            Assert.AreEqual(result[9], 0x20);

            // Coil count
            Assert.AreEqual(result[10], 0x00);
            Assert.AreEqual(result[11], 0x0C);
        }

        /// <summary>
        /// Example: Read 12 coils starting at 00033 from a PLC at address 2. Response with coils 00040 and 00042 set and all others clear :
        /// Response: 00 05 00 00 00 05 02 01 02 80 02
        /// </summary>
        [TestMethod]
        public void ParseReadCoilsResponse()
        {
            byte[] response = new byte[11] { 0x00, 0x05, 0x00, 0x00, 0x00, 0x05, 0x02, 0x01, 0x02, 0x80, 0x02 };
            bool[] values;
            string error;
            bool result = ModbusTcpProtocol.Current.ParseReadCoilsResponse(response, out values, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(values[7], true);
            Assert.AreEqual(values[9], true);
        }

        /// <summary>
        /// Example: Read 32 inputs starting at 10501 from a PLC at address 1. Response with inputs 10501 and 10503 set and all others clear :
        /// Request: 00 0A 00 00 00 06 01 02 01 F4 00 20
        /// </summary>
        [TestMethod]
        public void BuildReadInputsRequest()
        {
            byte[] expectResult = new byte[12] { 0x00, 0x0A, 0x00, 0x00, 0x00, 0x06, 0x01, 0x02, 0x01, 0xF4, 0x00, 0x20 };
            ModbusTcpProtocol.Current.TransactionId = 0x0A;
            byte[] result = ModbusTcpProtocol.Current.BuildReadInputsRequest(1, 500, 32);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Read 16 inputs starting at 10501 from a PLC at address 1. Response with inputs 10501 and 10503 set and all others clear :
        /// Response: 00 0A 00 00 00 05 01 02 02 05 00
        /// </summary>
        [TestMethod]
        public void ParseReadInputsResponse()
        {
            byte[] response = new byte[11] { 0x00, 0x0A, 0x00, 0x00, 0x00, 0x05, 0x01, 0x02, 0x02, 0x05, 0x00 };
            bool[] values;
            string error;
            bool result = ModbusTcpProtocol.Current.ParseReadInputResponse(response, out values, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(values[0], true);
            Assert.AreEqual(values[2], true);
        }

        /// <summary>
        /// Example: Read 2 input registers starting at address 30201 from a PLC at address 1. Response returns register 30201 value 10000, and register 30202 value 50000:
        /// Request: 00 0A 00 00 00 06 01 04 00 C8 00 02
        /// </summary>
        [TestMethod]
        public void BuildReadInputRegistersRequest()
        {
            byte[] expectResult = new byte[] { 0x00, 0x0A, 0x00, 0x00, 0x00, 0x06, 0x01, 0x04, 0x00, 0xC8, 0x00, 0x02 };
            ModbusTcpProtocol.Current.TransactionId = 0x0A;
            byte[] result = ModbusTcpProtocol.Current.BuildReadInputRegistersRequest(1, 200, 2);

            string text = ByteConverter.ToString(result);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Read 2 input registers starting at address 30201 from a PLC at address 1. Response returns register 30201 value 10000, and register 30202 value 50000:
        /// Response: 00 0A 00 00 00 07 01 04 04 27 10 C3 50
        /// </summary>
        [TestMethod]
        public void ParseReadInputRegistersResponse()
        {
            byte[] response = new byte[] { 0x00, 0x0A, 0x00, 0x00, 0x00, 0x07, 0x01, 0x04, 0x04, 0x27, 0x10, 0xC3, 0x50 };
            ushort[] values;
            string error;
            bool result = ModbusTcpProtocol.Current.ParseReadInputRegistersResponse(response, out values, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(values[0], 10000);
            Assert.AreEqual(values[1], 50000);
        }

        /// <summary>
        /// Example: Read 2 holding registers starting at address 40601 from a PLC at address 1. Response returns register 40601 value 1000, and register 40602 value 5000:
        /// Request: 00 0F 00 00 00 06 01 03 02 58 00 02
        /// </summary>
        [TestMethod]
        public void BuildReadHoldingRegistersRequest()
        {
            byte[] expectResult = new byte[12] { 0x00, 0x0F, 0x00, 0x00, 0x00, 0x06, 0x01, 0x03, 0x02, 0x58, 0x00, 0x02 };
            ModbusTcpProtocol.Current.TransactionId = 0x0F;
            byte[] result = ModbusTcpProtocol.Current.BuildReadHoldingRegistersRequest(1, 600, 2);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Read 2 holding registers starting at address 40601 from a PLC at address 1. Response returns register 40601 value 1000, and register 40602 value 5000:
        /// Response: 00 0F 00 00 00 07 01 03 04 03 E8 13 88
        /// </summary>
        [TestMethod]
        public void ParseReadHoldingRegistersResponse()
        {
            byte[] response = new byte[13] { 0x00, 0x0F, 0x00, 0x00, 0x00, 0x07, 0x01, 0x03, 0x04, 0x03, 0xE8, 0x13, 0x88 };
            ushort[] values;
            string error;
            bool result = ModbusTcpProtocol.Current.ParseReadHoldingRegistersResponse(response, out values, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(values[0], 1000);
            Assert.AreEqual(values[1], 5000);
        }

        /// <summary>
        /// Example: Set coil 00101 in device with address 1 to 1.
        /// Request: 00 19 00 00 00 06 01 05 00 64 FF 00
        /// </summary>
        [TestMethod]
        public void BuildWriteSingleCoilRequest()
        {
            byte[] expectResult = new byte[] { 0x00, 0x19, 0x00, 0x00, 0x00, 0x06, 0x01, 0x05, 0x00, 0x64, 0xFF, 0x00 };
            ModbusTcpProtocol.Current.TransactionId = 0x19;
            byte[] result = ModbusTcpProtocol.Current.BuildWriteSingleCoilRequest(1, 100, true);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Set coil 00101 in device with address 1 to 1.
        /// Response: 00 19 00 00 00 06 01 05 00 64 FF 00
        /// </summary>
        [TestMethod]
        public void ParseWriteSingleCoilResponse()
        {
            byte[] response = new byte[] { 0x00, 0x19, 0x00, 0x00, 0x00, 0x06, 0x01, 0x05, 0x00, 0x64, 0xFF, 0x00 };
            bool value;
            string error;
            bool result = ModbusTcpProtocol.Current.ParseWriteSingleCoilResponse(response, out value, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(value, true);
        }

        /// <summary>
        /// Example: Set Holding Register 40101 in device with address 1 to 15000.
        /// Request: 00 1E 00 00 00 06 01 06 00 64 3A 98
        /// </summary>
        [TestMethod]
        public void BuildWriteSingleRegisterRequest()
        {
            byte[] expectResult = new byte[] { 0x00, 0x1E, 0x00, 0x00, 0x00, 0x06, 0x01, 0x06, 0x00, 0x64, 0x3A, 0x98 };
            ModbusTcpProtocol.Current.TransactionId = 0x1E;
            byte[] result = ModbusTcpProtocol.Current.BuildWriteSingleRegisterRequest(1, 100, 15000);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Set Holding Register 40101 in device with address 1 to 15000.
        /// Response: 00 1E 00 00 00 06 01 06 00 64 3A 98
        /// </summary>
        [TestMethod]
        public void ParseWriteSingleRegisterResponse()
        {
            byte[] response = new byte[] { 0x00, 0x1E, 0x00, 0x00, 0x00, 0x06, 0x01, 0x06, 0x00, 0x64, 0x3A, 0x98 };
            ushort value;
            string error;
            bool result = ModbusTcpProtocol.Current.ParseWriteSingleRegisterResponse(response, out value, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(value, 15000);
        }

        /// <summary>
        /// Example: This command is writing the contents of a series of 10 discrete coils from #20 to #29 
        /// to the slave device with address 17.
        /// Request: 00 1E 00 00 00 09 11 0F 00 13 00 0A 02 CD 01
        /// </summary>
        [TestMethod]
        public void BuildWriteMultipleCoilsRequest()
        {
            byte[] expectResult = new byte[] { 0x00, 0x1E, 0x00, 0x00, 0x00, 0x09, 0x11, 0x0F, 0x00, 0x13, 0x00, 0x0A, 0x02, 0xCD, 0x01 };
            bool[] values = new bool[] { true, false, true, true, false, false, true, true, true, false };
            ModbusTcpProtocol.Current.TransactionId = 0x1E;
            byte[] result = ModbusTcpProtocol.Current.BuildWriteMultipleCoilsRequest(17, 19, values);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: This command is writing the contents of a series of 10 discrete coils from #20 to #29 
        /// to the slave device with address 17.
        /// Request: 00 1E 00 00 00 06 11 0F 00 13 00 0A
        /// </summary>
        [TestMethod]
        public void ParseWriteMultipleCoilsResponse()
        {
            byte[] response = new byte[] { 0x00, 0x1E, 0x00, 0x00, 0x00, 0x06, 0x11, 0x0F, 0x00, 0x13, 0x00, 0x0A };
            string error;
            bool result = ModbusTcpProtocol.Current.ParseWriteMultipleCoilsResponse(response, out error);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Example: In the PLC at address 28, set register 40101 to 1000 and register 40102 to 2000.
        /// Request: 00 23 00 00 00 0B 1C 10 00 64 00 02 04 03 E8 07 D0
        /// </summary>
        [TestMethod]
        public void BuildWriteMultipleRegistersRequest()
        {
            byte[] expectResult = new byte[] { 0x00, 0x23, 0x00, 0x00, 0x00, 0x0B, 0x1C, 0x10, 0x00, 0x64, 0x00, 0x02, 0x04, 0x03, 0xE8, 0x07, 0xD0 };
            ushort[] values = new ushort[] { 1000, 2000 };
            ModbusTcpProtocol.Current.TransactionId = 0x23;
            byte[] result = ModbusTcpProtocol.Current.BuildWriteMultipleRegistersRequest(28, 100, values);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: In the PLC at address 28, set register 40101 to 1000 and register 40102 to 2000.
        /// Response: 00 23 00 00 00 0B 1C 10 00 64 00 02 04 03 E8 07 D0
        /// </summary>
        [TestMethod]
        public void ParseWriteMultipleRegistersResponse()
        {
            byte[] response = new byte[] { 0x00, 0x23, 0x00, 0x00, 0x00, 0x06, 0x1C, 0x10, 0x00, 0x64, 0x00, 0x02 };
            string error;
            bool result = ModbusTcpProtocol.Current.ParseWriteMultipleRegistersResponse(response, out error);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseWriteMultipleRegisterResponse_BadLength()
        {
            byte[] response = new byte[] { 0x00, 0x23, 0x00, 0x00, 0x00, 0x06, 0x1C };
            string error;
            bool result = ModbusTcpProtocol.Current.ParseWriteMultipleRegistersResponse(response, out error);

            Assert.IsFalse(result);
            Assert.IsTrue(error.Contains("length", StringComparison.OrdinalIgnoreCase));
        }
    }
}
