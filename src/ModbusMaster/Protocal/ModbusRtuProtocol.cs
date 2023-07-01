using ModbusMaster.ByteUtils;
using System;

namespace ModbusMaster.Protocal
{
    public class ModbusRtuProtocol : ModbusProtocolBase
    {
        private static ModbusRtuProtocol _current;

        public ModbusRtuProtocol() : base(ProtocolFormat.RTU)
        {
        }

        public static ModbusRtuProtocol Current
        {
            get
            {
                if (_current == null)
                    _current = new ModbusRtuProtocol();
                return _current;
            }
        }

        // The structure of a Modbus RTU message is:
        // Slave Address: 1Byte, PDU: N Bytes, CRC: 2Bytes
        public override byte[] BuildRequest(byte slaveAddress, FunctionCode functionCode, ushort startAddress, ushort? numberOfPoints, byte[] data, ushort? extendedFileNumber = null)
        {
            ByteArrayBuilder byteArrayBuilder = new ByteArrayBuilder();

            byteArrayBuilder.Append(slaveAddress);
            byteArrayBuilder.Append(BuildPdu(functionCode, startAddress, numberOfPoints, data, extendedFileNumber));

            byte[] bytes = byteArrayBuilder.ToByteArray();
            byteArrayBuilder.Append(BitConverter.GetBytes(CheckSum.CalculateCrc(bytes)));

            return byteArrayBuilder.ToByteArray();
        }

        // The structure of a Modbus RTU message is:
        // Slave Address: 1Byte, PDU: N Bytes, CRC: 2Bytes
        public override bool ParseResponse(byte[] response, out FunctionCode functionCode, out byte[] data, out string error)
        {
            functionCode = 0;
            data = null;
            error = null;

            if (response.Length < 5)
            {
                error = $"Receive Length Error : {response.Length}.";
                return false;
            }

            // Error Check
            ushort crc = UInt16Converter.FromByteArray(response, response.Length - 2);
            ushort expectCrc = CheckSum.CalculateCrc(response, 0, response.Length - 2);
            if (crc != expectCrc)
            {
                error = $"Receive CRC Error.";
                return false;
            }

            // byte slaveAddress = response[0];

            byte[] pdu = ByteConverter.ToArray(response, 1, response.Length - 3);
            if (ParsePdu(pdu, out functionCode, out data))
            {
                return true;
            }

            return false;
        }
    }
}
