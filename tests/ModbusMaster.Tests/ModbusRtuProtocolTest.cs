using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModbusMaster.ByteUtils;
using ModbusMaster.Protocal;
using System;
using System.Linq;

namespace ModbusMaster.Tests
{
    [TestClass]
    public class ModbusRtuProtocolTest
    {
        /// <summary>
        /// Example: Read 12 coils starting at 00033 from a PLC at address 2. Response with coils 00040 and 00042 set and all others clear :
        /// Request: 02 01 00 20 00 0C 3D F6
        /// </summary>
        [TestMethod]
        public void BuildReadCoilsRequest()
        {
            byte[] expectResult = new byte[8] { 0x02, 0x01, 0x00, 0x20, 0x00, 0x0C, 0x3D, 0xF6 };
            byte[] result = ModbusRtuProtocol.Current.BuildReadCoilsRequest(2, 32, 12);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        [TestMethod]
        public void BuildReadCoils_0Points()
        {
            try
            {
                ModbusRtuProtocol.Current.BuildReadCoilsRequest(2, 32, 0);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentOutOfRangeException);
            }
        }

        [TestMethod]
        public void BuildReadCoilsRequest_2000Points()
        {
            string expectResult = "0x02, 0x01, 0x00, 0x20, 0x07, 0xD0, 0x3E, 0x5F";
            byte[] responce = ModbusRtuProtocol.Current.BuildReadCoilsRequest(2, 32, 2000);

            string result = ByteConverter.ToString(responce);
            Assert.AreEqual(result, expectResult);
        }

        [TestMethod]
        public void BuildReadCoilsRequest_2001Points()
        {
            try
            {
                ModbusRtuProtocol.Current.BuildReadCoilsRequest(2, 32, 2001);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentOutOfRangeException);
            }
        }

        /// <summary>
        /// Example: Read 12 coils starting at 00033 from a PLC at address 2. Response with coils 00040 and 00042 set and all others clear :
        /// Response: 02 01 02 80 02 1D FD
        /// </summary>
        [TestMethod]
        public void ParseReadCoilsResponse()
        {
            byte[] response = new byte[7] { 0x02, 0x01, 0x02, 0x80, 0x02, 0x1D, 0xFD };
            bool[] values;
            string error;
            bool result = ModbusRtuProtocol.Current.ParseReadCoilsResponse(response, out values, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(values[7], true);
            Assert.AreEqual(values[9], true);
        }

        [TestMethod]
        public void ParseReadCoilsResponse_BadLength()
        {
            byte[] response = new byte[] { 0x02, 0x01, 0x02, 0x80 };
            bool[] values;
            string error;
            bool result = ModbusRtuProtocol.Current.ParseReadCoilsResponse(response, out values, out error);

            Assert.IsFalse(result);
            Assert.IsTrue(error.Contains("length", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void ParseReadCoilsResponse_BadCrc()
        {
            byte[] response = new byte[7] { 0x02, 0x01, 0x02, 0x80, 0x02, 0x1D, 0xFF };
            bool[] values;
            string error;
            bool result = ModbusRtuProtocol.Current.ParseReadCoilsResponse(response, out values, out error);

            Assert.IsFalse(result);
            Assert.IsTrue(error.Contains("crc", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Example: Read 32 inputs starting at 10501 from a PLC at address 1. Response with inputs 10501 and 10503 set and all others clear :
        /// Request: 01 02 01 F4 00 20 39 DC
        /// </summary>
        [TestMethod]
        public void BuildReadInputsRequest()
        {
            byte[] expectResult = new byte[8] { 0x01, 0x02, 0x01, 0xF4, 0x00, 0x20, 0x39, 0xDC };
            byte[] result = ModbusRtuProtocol.Current.BuildReadInputsRequest(1, 500, 32);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Read 16 inputs starting at 10501 from a PLC at address 1. Response with inputs 10501 and 10503 set and all others clear :
        /// Response: 01 02 02 05 00 BA E8
        /// </summary>
        [TestMethod]
        public void ParseReadInputsResponse()
        {
            byte[] response = new byte[7] { 0x01, 0x02, 0x02, 0x05, 0x00, 0xBA, 0xE8 };
            bool[] values;
            string error;
            bool result = ModbusRtuProtocol.Current.ParseReadInputResponse(response, out values, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(values[0], true);
            Assert.AreEqual(values[2], true);
        }

        /// <summary>
        /// Example: Read 2 input registers starting at address 30201 from a PLC at address 1. Response returns register 30201 value 10000, and register 30202 value 50000:
        /// Request: 01 04 00 C8 00 02 F0 35
        /// </summary>
        [TestMethod]
        public void BuildReadInputRegistersRequest()
        {
            byte[] expectResult = new byte[] { 0x01, 0x04, 0x00, 0xC8, 0x00, 0x02, 0xF0, 0x35 };
            byte[] result = ModbusRtuProtocol.Current.BuildReadInputRegistersRequest(1, 200, 2);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Read 2 input registers starting at address 30201 from a PLC at address 1. Response returns register 30201 value 10000, and register 30202 value 50000:
        /// Response: 01 04 04 27 10 C3 50 A0 39
        /// </summary>
        [TestMethod]
        public void ParseReadInputRegistersResponse()
        {
            byte[] response = new byte[] { 0x01, 0x04, 0x04, 0x27, 0x10, 0xC3, 0x50, 0xA0, 0x39 };
            ushort[] values;
            string error;
            bool result = ModbusRtuProtocol.Current.ParseReadInputRegistersResponse(response, out values, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(values[0], 10000);
            Assert.AreEqual(values[1], 50000);
        }

        /// <summary>
        /// Example: Read 2 holding registers starting at address 40601 from a PLC at address 1. Response returns register 40601 value 1000, and register 40602 value 5000:
        /// Request: 01 03 02 58 00 02 44 60
        /// </summary>
        [TestMethod]
        public void BuildReadHoldingRegistersRequest()
        {
            byte[] expectResult = new byte[8] { 0x01, 0x03, 0x02, 0x58, 0x00, 0x02, 0x44, 0x60 };
            byte[] result = ModbusRtuProtocol.Current.BuildReadHoldingRegistersRequest(1, 600, 2);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        [TestMethod]
        public void BuildReadHoldingRegistersRequest_0Points()
        {
            try
            {
                ModbusRtuProtocol.Current.BuildReadHoldingRegistersRequest(1, 600, 0);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentOutOfRangeException);
            }
        }

        [TestMethod]
        public void BuildReadHoldingRegistersRequest_125Points()
        {
            string expectResult = "0x01, 0x03, 0x02, 0x58, 0x00, 0x7D, 0x05, 0x80";
            byte[] responce = ModbusRtuProtocol.Current.BuildReadHoldingRegistersRequest(1, 600, 125);

            string result = ByteConverter.ToString(responce);
            Assert.AreEqual(result, expectResult);
        }

        [TestMethod]
        public void BuildReadHoldingRegistersRequest_126Points()
        {
            try
            {
                ModbusRtuProtocol.Current.BuildReadHoldingRegistersRequest(1, 600, 126);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentOutOfRangeException);
            }
        }

        /// <summary>
        /// Example: Read 2 holding registers starting at address 40601 from a PLC at address 1. Response returns register 40601 value 1000, and register 40602 value 5000:
        /// Response: 01 03 04 03 E8 13 88 77 15
        /// </summary>
        [TestMethod]
        public void ParseReadHoldingRegistersResponse()
        {
            byte[] response = new byte[9] { 0x01, 0x03, 0x04, 0x03, 0xE8, 0x13, 0x88, 0x77, 0x15 };
            ushort[] values;
            string error;
            bool result = ModbusRtuProtocol.Current.ParseReadHoldingRegistersResponse(response, out values, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(values[0], 1000);
            Assert.AreEqual(values[1], 5000);
        }

        /// <summary>
        /// Group 1 consists of two registers from file 4, starting at register 1 (address 0001).
        /// 11 14 07 06 00 04 00 01 00 02 D9 70
        /// </summary>
        [TestMethod()]
        public void BuildReadExtendedRegistersRequestTest()
        {
            byte[] expectResult = new byte[] { 0x11, 0x14, 0x07, 0x06, 0x00, 0x04, 0x00, 0x01, 0x00, 0x02, 0xD9, 0x70 };
            byte[] result = ModbusRtuProtocol.Current.BuildReadExtendedRegistersRequest(17, 1, 2, 4);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Group 1 consists of two registers from file 4, starting at register 1 (address 0001).
        /// 11 14 06 05 06 0D FE 00 20 46 8E
        /// </summary>
        [TestMethod()]
        public void ParseRReadExtendedRegistersResponseTest()
        {
            byte[] response = new byte[] { 0x11, 0x14, 0x06, 0x05, 0x06, 0x0D, 0xFE, 0x00, 0x20, 0x46, 0x8E };
            ushort[] values;
            string error;
            bool result = ModbusRtuProtocol.Current.ParseRReadExtendedRegistersResponse(response, out values, out error);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Example: Set coil 00101 in device with address 1 to 1.
        /// Request: 01 05 00 64 FF 00 CD E5
        /// </summary>
        [TestMethod]
        public void BuildWriteSingleCoilRequest()
        {
            byte[] expectResult = new byte[] { 0x01, 0x05, 0x00, 0x64, 0xFF, 0x00, 0xCD, 0xE5 };
            byte[] result = ModbusRtuProtocol.Current.BuildWriteSingleCoilRequest(1, 100, true);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Set coil 00101 in device with address 1 to 1.
        /// Response: 01 05 00 64 FF 00 CD E5
        /// </summary>
        [TestMethod]
        public void ParseWriteSingleCoilResponse()
        {
            byte[] response = new byte[] { 0x01, 0x05, 0x00, 0x64, 0xFF, 0x00, 0xCD, 0xE5 };
            bool value;
            string error;
            bool result = ModbusRtuProtocol.Current.ParseWriteSingleCoilResponse(response, out value, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(value, true);
        }

        /// <summary>
        /// Example: This command is writing the contents of a series of 10 discrete coils from #20 to #29 
        /// to the slave device with address 17.
        /// Request: 11 0F 00 13 00 0A 02 CD 01 BF 0B
        /// </summary>
        [TestMethod]
        public void BuildWriteMultipleCoilsRequest()
        {
            byte[] expectResult = new byte[] { 0x11, 0x0F, 0x00, 0x13, 0x00, 0x0A, 0x02, 0xCD, 0x01, 0xBF, 0x0B };
            bool[] values = new bool[] { true, false, true, true, false, false, true, true, true, false };
            byte[] result = ModbusRtuProtocol.Current.BuildWriteMultipleCoilsRequest(17, 19, values);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Set coil 00101 in device with address 1 to 1.
        /// Response: 11 0F 00 13 00 0A 26 99
        /// </summary>
        [TestMethod]
        public void ParseWriteMultipleCoilsResponse()
        {
            byte[] response = new byte[] { 0x11, 0x0F, 0x00, 0x13, 0x00, 0x0A, 0x26, 0x99 };
            string error;
            bool result = ModbusRtuProtocol.Current.ParseWriteMultipleCoilsResponse(response, out error);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Example: Set Holding Register 40101 in device with address 1 to 15000.
        /// Request: 01 06 00 64 3A 98 DB 1F
        /// </summary>
        [TestMethod]
        public void BuildWriteSingleRegisterRequest()
        {
            byte[] expectResult = new byte[] { 0x01, 0x06, 0x00, 0x64, 0x3A, 0x98, 0xDB, 0x1F };
            byte[] result = ModbusRtuProtocol.Current.BuildWriteSingleRegisterRequest(1, 100, 15000);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example:  Set Holding Register 40101 in device with address 1 to 15000.
        /// Response: 01 06 00 64 3A 98 DB 1F
        /// </summary>
        [TestMethod]
        public void ParseWriteSingleRegisterResponse()
        {
            byte[] response = new byte[] { 0x01, 0x06, 0x00, 0x64, 0x3A, 0x98, 0xDB, 0x1F };
            ushort value;
            string error;
            bool result = ModbusRtuProtocol.Current.ParseWriteSingleRegisterResponse(response, out value, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(value, 15000);
        }

        /// <summary>
        /// Example: In the PLC at address 28, set register 40101 to 1000 and register 40102 to 2000.
        /// Request: 1C 10 00 64 00 02 04 03 E8 07 D0 18 C4
        /// </summary>
        [TestMethod]
        public void BuildWriteMultipleRegistersRequest()
        {
            byte[] expectResult = new byte[] { 0x1C, 0x10, 0x00, 0x64, 0x00, 0x02, 0x04, 0x03, 0xE8, 0x07, 0xD0, 0x18, 0xC4 };
            ushort[] values = new ushort[] { 1000, 2000 };
            byte[] result = ModbusRtuProtocol.Current.BuildWriteMultipleRegistersRequest(28, 100, values);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: In the PLC at address 28, set register 40101 to 1000 and register 40102 to 2000.
        /// Response: 1C 10 00 64 00 02 03 9A
        /// </summary>
        [TestMethod]
        public void ParseWriteMultipleRegistersResponse()
        {
            byte[] response = new byte[] { 0x1C, 0x10, 0x00, 0x64, 0x00, 0x02, 0x03, 0x9A };
            string error;
            bool result = ModbusRtuProtocol.Current.ParseWriteMultipleRegistersResponse(response, out error);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// An example of a request to write one group of references into slave device 17 is shown below.
        /// The group consists of three registers in file 4, starting at register 7 (address 0007).
        /// Request: 11 15 0D 06 00 04 00 07 00 03 06 AF 04 BE 10 0D DB C7
        /// </summary>
        [TestMethod()]
        public void BuildWriteExtendedRegistersRequestTest()
        {
            byte[] expectResult = new byte[] { 0x11, 0x15, 0x0D, 0x06, 0x00, 0x04, 0x00, 0x07, 0x00, 0x03, 0x06, 0xAF, 0x04, 0xBE, 0x10, 0x0D, 0xDB, 0xC7 };
            ushort[] values = new ushort[] { 0x06AF, 0x04BE, 0x100D };
            byte[] result = ModbusRtuProtocol.Current.BuildWriteExtendedRegistersRequest(17, 7, values, 4);

            //string text = BitConverter.ToString(result).Replace("-", " ");
            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// An example of a request to write one group of references into slave device 17 is shown below.
        /// The group consists of three registers in file 4, starting at register 7 (address 0007).
        /// Response: 11 15 0D 06 00 04 00 07 00 03 06 AF 04 BE 10 0D DB C7
        /// </summary>
        [TestMethod()]
        public void ParseWriteExtendedRegistersResponseTest()
        {
            byte[] response = new byte[] { 0x11, 0x15, 0x0D, 0x06, 0x00, 0x04, 0x00, 0x07, 0x00, 0x03, 0x06, 0xAF, 0x04, 0xBE, 0x10, 0x0D, 0xDB, 0xC7 };
            string error;
            bool result = ModbusRtuProtocol.Current.ParseWriteExtendedRegistersResponse(response, out error);

            Assert.IsTrue(result);
        }


    }
}
