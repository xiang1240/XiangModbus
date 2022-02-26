using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModbusMaster.Protocal;
using System;
using System.Linq;

namespace ModbusMaster.Tests
{
    [TestClass]
    public class ModbusAsciiProtocolTest
    {
        /// <summary>
        /// Example: Read 12 coils starting at 00033 from a PLC at address 2. Response with coils 00040 and 00042 set and all others clear :
        /// Request: 3A 30 32 30 31 30 30 32 30 30 30 30 43 44 31 0D 0A
        /// </summary>
        [TestMethod]
        public void BuildReadCoilsRequest()
        {
            byte[] expectResult = new byte[17] { 0x3A, 0x30, 0x32, 0x30, 0x31, 0x30, 0x30, 0x32, 0x30, 0x30, 0x30, 0x30, 0x43, 0x44, 0x31, 0x0D, 0x0A };
            byte[] result = ModbusAsciiProtocol.Current.BuildReadCoilsRequest(2, 32, 12);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Read 12 coils starting at 00033 from a PLC at address 2. Response with coils 00040 and 00042 set and all others clear :
        /// Response: 3A 30 32 30 31 30 32 38 30 30 32 37 39 0D 0A
        /// </summary>
        [TestMethod]
        public void ParseReadCoilsResponse()
        {
            byte[] response = new byte[15] { 0x3A, 0x30, 0x32, 0x30, 0x31, 0x30, 0x32, 0x38, 0x30, 0x30, 0x32, 0x37, 0x39, 0x0D, 0x0A };
            bool[] values;
            string error;
            bool result = ModbusAsciiProtocol.Current.ParseReadCoilsResponse(response, out values, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(values[7], true);
            Assert.AreEqual(values[9], true);
        }

        /// <summary>
        /// Example: Read 32 inputs starting at 10501 from a PLC at address 1. Response with inputs 10501 and 10503 set and all others clear :
        /// Request: 3A 30 31 30 32 30 31 46 34 30 30 32 30 45 38 0D 0A
        /// </summary>
        [TestMethod]
        public void BuildReadInputsRequest()
        {
            byte[] expectResult = new byte[17] { 0x3A, 0x30, 0x31, 0x30, 0x32, 0x30, 0x31, 0x46, 0x34, 0x30, 0x30, 0x32, 0x30, 0x45, 0x38, 0x0D, 0x0A };
            byte[] result = ModbusAsciiProtocol.Current.BuildReadInputsRequest(1, 500, 32);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Read 16 inputs starting at 10501 from a PLC at address 1. Response with inputs 10501 and 10503 set and all others clear :
        /// Response: 3A 30 31 30 32 30 32 30 35 30 30 46 36 0D 0A
        /// </summary>
        [TestMethod]
        public void ParseReadInputsResponse()
        {
            byte[] response = new byte[15] { 0x3A, 0x30, 0x31, 0x30, 0x32, 0x30, 0x32, 0x30, 0x35, 0x30, 0x30, 0x46, 0x36, 0x0D, 0x0A };
            bool[] values;
            string error;
            bool result = ModbusAsciiProtocol.Current.ParseReadInputResponse(response, out values, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(values[0], true);
            Assert.AreEqual(values[2], true);
        }

        /// <summary>
        /// Example: Read 2 input registers starting at address 30201 from a PLC at address 1. Response returns register 30201 value 10000, and register 30202 value 50000:
        /// Request: 3A 30 31 30 34 30 30 43 38 30 30 30 32 33 31 0D 0A
        /// </summary>
        [TestMethod]
        public void BuildReadInputRegistersRequest()
        {
            byte[] expectResult = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x34, 0x30, 0x30, 0x43, 0x38, 0x30, 0x30, 0x30, 0x32, 0x33, 0x31, 0x0D, 0x0A };
            byte[] result = ModbusAsciiProtocol.Current.BuildReadInputRegistersRequest(1, 200, 2);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Read 2 input registers starting at address 30201 from a PLC at address 1. Response returns register 30201 value 10000, and register 30202 value 50000:
        /// Response: 3A 30 31 30 34 30 34 32 37 31 30 43 33 35 30 41 44 0D 0A
        /// </summary>
        [TestMethod]
        public void ParseReadInputRegistersResponse()
        {
            byte[] response = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x34, 0x30, 0x34, 0x32, 0x37, 0x31, 0x30, 0x43, 0x33, 0x35, 0x30, 0x41, 0x44, 0x0D, 0x0A };
            ushort[] values;
            string error;
            bool result = ModbusAsciiProtocol.Current.ParseReadInputRegistersResponse(response, out values, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(values[0], 10000);
            Assert.AreEqual(values[1], 50000);
        }

        /// <summary>
        /// Example: Read 2 holding registers starting at address 40601 from a PLC at address 1. Response returns register 40601 value 1000, and register 40602 value 5000:
        /// Request: 3A 30 31 30 33 30 32 35 38 30 30 30 32 41 30 0D 0A
        /// </summary>
        [TestMethod]
        public void BuildReadHoldingRegistersRequest()
        {
            byte[] expectResult = new byte[17] { 0x3A, 0x30, 0x31, 0x30, 0x33, 0x30, 0x32, 0x35, 0x38, 0x30, 0x30, 0x30, 0x32, 0x41, 0x30, 0x0D, 0x0A };
            byte[] result = ModbusAsciiProtocol.Current.BuildReadHoldingRegistersRequest(1, 600, 2);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Read 2 holding registers starting at address 40601 from a PLC at address 1. Response returns register 40601 value 1000, and register 40602 value 5000:
        /// Response: 3A 30 31 30 33 30 34 30 33 45 38 31 33 38 38 37 32 0D 0A
        /// </summary>
        [TestMethod]
        public void ParseReadHoldingRegistersResponse()
        {
            byte[] response = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x33, 0x30, 0x34, 0x30, 0x33, 0x45, 0x38, 0x31, 0x33, 0x38, 0x38, 0x37, 0x32, 0x0D, 0x0A };
            ushort[] values;
            string error;
            bool result = ModbusAsciiProtocol.Current.ParseReadHoldingRegistersResponse(response, out values, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(values[0], 1000);
            Assert.AreEqual(values[1], 5000);
        }

        [TestMethod]
        public void ParseReadHoldingRegistersResponse_BadLength()
        {
            byte[] response = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x33, 0x30 };
            bool[] values;
            string error;
            bool result = ModbusAsciiProtocol.Current.ParseReadCoilsResponse(response, out values, out error);

            Assert.IsFalse(result);
            Assert.IsTrue(error.Contains("length", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void ParseReadHoldingRegistersResponse_BadStartChar()
        {
            byte[] response = new byte[] { 0x3B, 0x30, 0x31, 0x30, 0x33, 0x30, 0x34, 0x30, 0x33, 0x45, 0x38, 0x31, 0x33, 0x38, 0x38, 0x37, 0x32, 0x0D, 0x0A };
            ushort[] values;
            string error;
            bool result = ModbusAsciiProtocol.Current.ParseReadHoldingRegistersResponse(response, out values, out error);

            Assert.IsFalse(result);
            Assert.IsTrue(error.Contains("start", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void ParseReadHoldingRegistersResponse_BadLrc()
        {
            byte[] response = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x33, 0x30, 0x34, 0x30, 0x33, 0x45, 0x38, 0x31, 0x33, 0x38, 0x38, 0x37, 0x33, 0x0D, 0x0A };
            bool[] values;
            string error;
            bool result = ModbusAsciiProtocol.Current.ParseReadCoilsResponse(response, out values, out error);

            Assert.IsFalse(result);
            Assert.IsTrue(error.Contains("lrc", StringComparison.OrdinalIgnoreCase));
        }

        // <summary>
        /// Example: Set coil 00101 in device with address 1 to 1.
        /// Request: 3A 30 31 30 35 30 30 36 34 46 46 30 30 39 37 0D 0A
        /// </summary>
        [TestMethod]
        public void BuildWriteSingleCoilRequest()
        {
            byte[] expectResult = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x30, 0x36, 0x34, 0x46, 0x46, 0x30, 0x30, 0x39, 0x37, 0x0D, 0x0A };
            byte[] result = ModbusAsciiProtocol.Current.BuildWriteSingleCoilRequest(1, 100, true);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Set coil 00101 in device with address 1 to 1.
        /// Response: 3A 30 31 30 35 30 30 36 34 46 46 30 30 39 37 0D 0A
        /// </summary>
        [TestMethod]
        public void ParseWriteSingleCoilResponse()
        {
            byte[] response = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x35, 0x30, 0x30, 0x36, 0x34, 0x46, 0x46, 0x30, 0x30, 0x39, 0x37, 0x0D, 0x0A };
            bool value;
            string error;
            bool result = ModbusAsciiProtocol.Current.ParseWriteSingleCoilResponse(response, out value, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(value, true);
        }

        /// <summary>
        /// Example: Set Holding Register 40101 in device with address 1 to 15000.
        /// Request: 3A 30 31 30 36 30 30 36 34 33 41 39 38 43 33 0D 0A
        /// </summary>
        [TestMethod]
        public void BuildWriteSingleRegisterRequest()
        {
            byte[] expectResult = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x36, 0x30, 0x30, 0x36, 0x34, 0x33, 0x41, 0x39, 0x38, 0x43, 0x33, 0x0D, 0x0A };
            byte[] result = ModbusAsciiProtocol.Current.BuildWriteSingleRegisterRequest(1, 100, 15000);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: Set Holding Register 40101 in device with address 1 to 15000.
        /// Response: 3A 30 31 30 36 30 30 36 34 33 41 39 38 43 33 0D 0A
        /// </summary>
        [TestMethod]
        public void ParseWriteSingleRegisterResponse()
        {
            byte[] response = new byte[] { 0x3A, 0x30, 0x31, 0x30, 0x36, 0x30, 0x30, 0x36, 0x34, 0x33, 0x41, 0x39, 0x38, 0x43, 0x33, 0x0D, 0x0A };
            ushort value;
            string error;
            bool result = ModbusAsciiProtocol.Current.ParseWriteSingleRegisterResponse(response, out value, out error);

            Assert.IsTrue(result);
            Assert.AreEqual(value, 15000);
        }

        /// <summary>
        /// Example: In the PLC at address 28, set register 40101 to 1000 and register 40102 to 2000.
        /// Request: 3A 31 43 31 30 30 30 36 34 30 30 30 32 30 34  30 33 45 38  30 37 44 30  41 38  0D 0A
        /// </summary>
        [TestMethod]
        public void BuildWriteMultipleRegistersRequest()
        {
            byte[] expectResult = new byte[] { 0x3A, 0x31, 0x43, 0x31, 0x30, 0x30, 0x30, 0x36, 0x34, 0x30, 0x30, 0x30, 0x32, 0x30, 0x34, 0x30, 0x33, 0x45, 0x38, 0x30, 0x37, 0x44, 0x30, 0x41, 0x38, 0x0D, 0x0A };
            ushort[] values = new ushort[] { 1000, 2000 };
            byte[] result = ModbusAsciiProtocol.Current.BuildWriteMultipleRegistersRequest(28, 100, values);

            Assert.IsTrue(Enumerable.SequenceEqual(result, expectResult));
        }

        /// <summary>
        /// Example: In the PLC at address 28, set register 40101 to 1000 and register 40102 to 2000.
        /// Response: 3A 31 43 31 30 30 30 36 34 30 30 30 32 36 45 0D 0A
        /// </summary>
        [TestMethod]
        public void ParseWriteMultipleRegistersResponse()
        {
            byte[] response = new byte[] { 0x3A, 0x31, 0x43, 0x31, 0x30, 0x30, 0x30, 0x36, 0x34, 0x30, 0x30, 0x30, 0x32, 0x36, 0x45, 0x0D, 0x0A };
            string error;
            bool result = ModbusAsciiProtocol.Current.ParseWriteMultipleRegistersResponse(response, out error);

            Assert.IsTrue(result);
        }
    }
}
