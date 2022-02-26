using ModbusMaster.ByteUtils;
using System;
using System.IO;
using System.Text;

namespace ModbusMaster.Protocal
{

    public class ModbusAsciiProtocol : ModbusProtocolBase
    {
        private const char STX = ':';
        private const char ReturnChar = '\r';
        private const char NewLineChar = '\n';
        private const string NewLine = "\r\n";

        private static ModbusAsciiProtocol _current;

        public ModbusAsciiProtocol() : base(ProtocolFormat.ASCII)
        {
        }

        public static ModbusAsciiProtocol Current
        {
            get
            {
                if (_current == null)
                    _current = new ModbusAsciiProtocol();
                return _current;
            }
        }

        // The structure of a Modbus ASCII message is:
        // Start: ASCII 58, Slave Address: 2 characters, Message: N characters, LRC: 2 characters, End: ASCII 13 + ASCII 10
        public override byte[] BuildRequest(byte slaveAddress, FunctionCode functionCode, ushort startAddress, ushort? numberOfPoints, byte[] data, ushort? extendedFileNumber = null)
        {
            ByteArrayBuilder byteArrayBuilder = new ByteArrayBuilder();

            byteArrayBuilder.Append(slaveAddress);
            byteArrayBuilder.Append(BuildPdu(functionCode, startAddress, numberOfPoints, data, extendedFileNumber));
            byte[] bytes = byteArrayBuilder.ToByteArray();

            StringWriter stringWriter = new StringWriter();
            stringWriter.Write(STX);
            stringWriter.Write(HexConverter.ToHexChars(bytes));
            byte lrc = CheckSum.CalculateLrc(bytes); // Before converting to ASCII and without the initial colon and final CR/LF
            //stringWriter.Write(lrc.ToString("X2"));
            stringWriter.Write(HexConverter.ToHexChars(lrc));

            stringWriter.Write(NewLine);
            string text = stringWriter.ToString();

            return Encoding.ASCII.GetBytes(text);
        }

        // The structure of a Modbus ASCII message is:
        // Start: ASCII 58, Slave Address: 2 characters, Message: N characters, LRC: 2 characters, End: ASCII 13 + ASCII 10
        public override bool ParseResponse(byte[] response, out FunctionCode functionCode, out byte[] data, out string error)
        {
            functionCode = 0;
            data = null;
            error = null;

            if (response.Length < 7)
            {
                error = $"Receive Length Error : {response.Length}.";
                return false;
            }

            if (response[0] != STX || response[response.Length - 2] != ReturnChar || response[response.Length - 1] != NewLineChar)
            {
                error = $"ASCII Start Or End Char Error : {response.Length}.";
                return false;
            }

            byte[] charBytes = new byte[response.Length - 3];
            Array.Copy(response, 1, charBytes, 0, response.Length - 3);
            byte[] bytes = HexConverter.ToBytesFromHexCharBytes(charBytes);

            // Error Check
            byte lrc = CheckSum.CalculateLrc(bytes, 0, bytes.Length - 1);
            if (bytes[bytes.Length - 1] != lrc)
            {
                error = $"Receive LRC Error.";
                return false;
            }

            // byte slaveAddress = bytes[0];

            byte[] pdu = ByteConverter.ToArray(bytes, 1, bytes.Length - 2);
            if (ParsePdu(pdu, out functionCode, out data))
            {
                return true;
            }

            return false;
        }
    }
}
