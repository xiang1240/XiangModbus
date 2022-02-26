using ModbusMaster.ByteUtils;

namespace ModbusMaster.Protocal
{
    public class ModbusTcpProtocol : ModbusProtocolBase
    {
        private static ModbusTcpProtocol _current;

        private ushort _transactionId;

        public ModbusTcpProtocol() : base(ProtocolFormat.TCP)
        {
        }

        public static ModbusTcpProtocol Current
        {
            get
            {
                if (_current == null)
                    _current = new ModbusTcpProtocol();
                return _current;
            }
        }

        public ushort TransactionId
        {
            get
            {
                return _transactionId;
            }
            set
            {
                _transactionId = value;
            }
        }

        // The structure of a Modbus TCP message is:
        // Transaction Id: 2 Bytes, Protocol: 2 Bytes, Length: 2 Bytes, Slave Address: 1 Byte, Message: N Bytes
        public override byte[] BuildRequest(byte slaveAddress, FunctionCode functionCode, ushort startAddress, ushort? numberOfPoints, byte[] data, ushort? extendedFileNumber = null)
        {
            ByteArrayBuilder byteArrayBuilder = new ByteArrayBuilder();

            byte[] pdu = BuildPdu(functionCode, startAddress, numberOfPoints, data, extendedFileNumber);
            byte[] header = BuildMBAPHeader(_transactionId, (ushort)(pdu.Length + 1), slaveAddress);

            byteArrayBuilder.Append(header);
            byteArrayBuilder.Append(pdu);

            return byteArrayBuilder.ToByteArray();
        }

        /// <summary>
        /// Build MBAP header(MODBUS Application Protocol header).
        /// </summary>
        public static byte[] BuildMBAPHeader(ushort transactionId, ushort length, byte slaveAddress)
        {
            ByteArrayBuilder byteArrayBuilder = new ByteArrayBuilder();

            // Transaction Identifier, Initialized by the client, Recopied by the server from the received
            byteArrayBuilder.Append(transactionId, false);
            // Protocol Identifier, 0 = MODBUS protocol 
            byteArrayBuilder.Append((ushort)0, false);
            // Length, Number of following bytes
            byteArrayBuilder.Append(length, false);
            // Slave Address
            byteArrayBuilder.Append(slaveAddress);

            return byteArrayBuilder.ToByteArray();
        }

        // The structure of a Modbus TCP message is:
        // Transaction Id: 2 Bytes, Protocol: 2 Bytes, Length: 2 Bytes, Slave Address: 1 Byte, Message: N Bytes
        public override bool ParseResponse(byte[] response, out FunctionCode functionCode, out byte[] data, out string error)
        {
            functionCode = 0;
            data = null;
            error = null;

            if (response.Length < 8)
            {
                error = $"Receive Length Error : {response.Length}.";
                return false;
            }

            ushort transactionId;
            ushort length;
            byte slaveAddress;
            if (ParseMBAPHeader(response, out transactionId, out length, out slaveAddress))
            {
                if (response.Length == length + 6) // The length of the entire message
                {
                    byte[] pdu = ByteConverter.ToArray(response, 7, length - 1);
                    if (ParsePdu(pdu, out functionCode, out data))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Parse MBAP header(MODBUS Application Protocol header).
        /// </summary>
        private static bool ParseMBAPHeader(byte[] response, out ushort transactionId, out ushort length, out byte slaveAddress)
        {
            transactionId = 0;
            length = 0;
            slaveAddress = 0;

            if (response.Length < 7)
                return false;

            // Transaction Identifier, Initialized by the client, Recopied by the server from the received
            transactionId = UInt16Converter.FromByteArray(response, 0, false);
            // Length, Number of following bytes
            length = UInt16Converter.FromByteArray(response, 4, false);
            // Slave Address
            slaveAddress = response[6];

            return true;
        }
    }
}
