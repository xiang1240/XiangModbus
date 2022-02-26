using ModbusMaster.ByteUtils;
using System;

namespace ModbusMaster.Protocal
{
    public abstract class ModbusProtocolBase
    {
        protected ProtocolFormat _protocolFormat;

        protected ModbusProtocolBase(ProtocolFormat protocolFormat)
        {
            _protocolFormat = protocolFormat;
        }

        public byte[] BuildReadCoilsRequest(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 2000);
            return BuildRequest(slaveAddress, FunctionCode.ReadCoils, startAddress, numberOfPoints, null);
        }

        public byte[] BuildReadInputsRequest(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 2000);
            return BuildRequest(slaveAddress, FunctionCode.ReadInputs, startAddress, numberOfPoints, null);
        }

        public byte[] BuildReadInputRegistersRequest(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 125);
            return BuildRequest(slaveAddress, FunctionCode.ReadInputRegisters, startAddress, numberOfPoints, null);
        }

        public byte[] BuildReadHoldingRegistersRequest(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 125);
            return BuildRequest(slaveAddress, FunctionCode.ReadHoldingRegisters, startAddress, numberOfPoints, null);
        }

        public byte[] BuildReadExtendedRegistersRequest(byte slaveAddress, ushort startAddress, ushort numberOfPoints, ushort fileNumber)
        {
            ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 125);
            return BuildRequest(slaveAddress, FunctionCode.ReadExtendedRegisters, startAddress, numberOfPoints, null, fileNumber);
        }

        public byte[] BuildWriteSingleCoilRequest(byte slaveAddress, ushort coilAddress, bool value)
        {
            var data = new byte[2];
            if (value)
                data[0] = 0xFF;
            return BuildRequest(slaveAddress, FunctionCode.WriteSingleCoil, coilAddress, null, data);
        }

        public byte[] BuildWriteMultipleCoilsRequest(byte slaveAddress, ushort startAddress, bool[] values)
        {
            ValidateData("data", values, 1968);

            byte[] data = BoolConverter.ToByteArray(values);
            return BuildRequest(slaveAddress, FunctionCode.WriteMultipleCoils, startAddress, (ushort)values.Length, data);
        }

        public byte[] BuildWriteSingleRegisterRequest(byte slaveAddress, ushort registerAddress, ushort value)
        {
            byte[] data = UInt16Converter.ToByteArray(value, false);
            return BuildRequest(slaveAddress, FunctionCode.WriteSingleRegister, registerAddress, null, data);
        }

        public byte[] BuildWriteMultipleRegistersRequest(byte slaveAddress, ushort startAddress, ushort[] values)
        {
            ValidateData("data", values, 123);
            byte[] bytes = UInt16Converter.ToByteArray(values, false);
            return BuildRequest(slaveAddress, FunctionCode.WriteMultipleRegisters, startAddress, (byte)values.Length, bytes);
        }

        public byte[] BuildWriteExtendedRegistersRequest(byte slaveAddress, ushort startAddress, ushort[] values, ushort fileNumber)
        {
            ValidateData("data", values, 123);
            byte[] bytes = UInt16Converter.ToByteArray(values, false);
            return BuildRequest(slaveAddress, FunctionCode.WriteExtendedRegisters, startAddress, (byte)values.Length, bytes, fileNumber);
        }

        public abstract byte[] BuildRequest(byte slaveAddress, FunctionCode functionCode, ushort startAddress, ushort? numberOfPoints, byte[] data, ushort? extendedFileNumber = null);

        public static byte[] BuildPdu(FunctionCode functionCode, ushort startAddress, ushort? numberOfPoints, byte[] data, ushort? extendedFileNumber = null)
        {
            ByteArrayBuilder byteArrayBuilder = new ByteArrayBuilder();

            byteArrayBuilder.Append((byte)functionCode);

            if (extendedFileNumber.HasValue)
            {
                byte byteCount = 7;
                if (data != null)
                    byteCount += (byte)data.Length;
                byteArrayBuilder.Append(byteCount); // Only extended file Registers has this field.


                byte referenceType = 6; // The reference type: 1 byte (must be specified as 6)
                byteArrayBuilder.Append(referenceType);

                byteArrayBuilder.Append(extendedFileNumber.Value, false); // The Extended Memory file number: 2 bytes(1 to 10, hex 0001 to 000A)
            }

            byteArrayBuilder.Append(startAddress, false);
            if (numberOfPoints.HasValue)
            {
                byteArrayBuilder.Append(numberOfPoints.Value, false);
            }
            if (data != null)
            {
                if (!(functionCode == FunctionCode.WriteSingleCoil || functionCode == FunctionCode.WriteSingleRegister || extendedFileNumber.HasValue))
                {
                    byteArrayBuilder.Append((byte)data.Length);
                }
                byteArrayBuilder.Append(data);
            }

            return byteArrayBuilder.ToByteArray();
        }

        public bool ParseReadCoilsResponse(byte[] response, out bool[] values, out string error)
        {
            return ParseReadBitResponse(response, FunctionCode.ReadCoils, out values, out error);
        }

        public bool ParseReadInputResponse(byte[] response, out bool[] values, out string error)
        {
            return ParseReadBitResponse(response, FunctionCode.ReadInputs, out values, out error);
        }

        public bool ParseReadInputRegistersResponse(byte[] response, out ushort[] values, out string error)
        {
            return ParseReadUInt16Response(response, FunctionCode.ReadInputRegisters, out values, out error);
        }

        public bool ParseReadHoldingRegistersResponse(byte[] response, out ushort[] values, out string error)
        {
            return ParseReadUInt16Response(response, FunctionCode.ReadHoldingRegisters, out values, out error);
        }

        public bool ParseRReadExtendedRegistersResponse(byte[] response, out ushort[] values, out string error)
        {
            return ParseReadUInt16Response(response, FunctionCode.ReadExtendedRegisters, out values, out error);
        }

        public bool ParseWriteSingleCoilResponse(byte[] response, out bool value, out string error)
        {
            value = false;
            FunctionCode responseFunctionCode;
            byte[] data;

            if (ParseResponse(response, out responseFunctionCode, out data, out error) && responseFunctionCode == FunctionCode.WriteSingleCoil)
            {
                if (data.Length == 2 && data[0] == 0xFF)
                    value = true;
                return true;
            }

            return false;
        }

        public bool ParseWriteMultipleCoilsResponse(byte[] response, out string error)
        {
            FunctionCode responseFunctionCode;
            byte[] data;

            if (ParseResponse(response, out responseFunctionCode, out data, out error) && responseFunctionCode == FunctionCode.WriteMultipleCoils)
            {
                return true;
            }

            return false;
        }

        public bool ParseWriteSingleRegisterResponse(byte[] response, out ushort value, out string error)
        {
            value = 0;
            FunctionCode responseFunctionCode;
            byte[] data;

            if (ParseResponse(response, out responseFunctionCode, out data, out error) && responseFunctionCode == FunctionCode.WriteSingleRegister)
            {
                value = UInt16Converter.FromByteArray(data, false);
                return true;
            }

            return false;
        }

        public bool ParseWriteMultipleRegistersResponse(byte[] response, out string error)
        {
            FunctionCode responseFunctionCode;
            byte[] data;

            if (ParseResponse(response, out responseFunctionCode, out data, out error) && responseFunctionCode == FunctionCode.WriteMultipleRegisters)
            {
                return true;
            }

            return false;
        }

        public bool ParseWriteExtendedRegistersResponse(byte[] response, out string error)
        {
            FunctionCode responseFunctionCode;
            byte[] data;

            if (ParseResponse(response, out responseFunctionCode, out data, out error) && responseFunctionCode == FunctionCode.WriteExtendedRegisters)
            {
                return true;
            }

            return false;
        }

        private bool ParseReadBitResponse(byte[] response, FunctionCode functionCode, out bool[] values, out string error)
        {
            values = null;
            FunctionCode responseFunctionCode;
            byte[] data;

            if (ParseResponse(response, out responseFunctionCode, out data, out error) && responseFunctionCode == functionCode)
            {
                values = BoolConverter.ToArray(data);
                return true;
            }

            return false;
        }

        private bool ParseReadUInt16Response(byte[] response, FunctionCode functionCode, out ushort[] values, out string error)
        {
            values = null;
            FunctionCode responseFunctionCode;
            byte[] data;

            if (ParseResponse(response, out responseFunctionCode, out data, out error) && responseFunctionCode == functionCode)
            {
                values = UInt16Converter.ToArray(data, false);
                return true;
            }

            return false;
        }

        public abstract bool ParseResponse(byte[] response, out FunctionCode functionCode, out byte[] data, out string error);

        public bool ParsePdu(byte[] pdu, out FunctionCode functionCode, out byte[] data)
        {
            data = null;
            byte byteCount;

            functionCode = (FunctionCode)pdu[0];

            switch (functionCode)
            {
                case FunctionCode.ReadCoils:
                case FunctionCode.ReadInputs:
                case FunctionCode.ReadHoldingRegisters:
                case FunctionCode.ReadInputRegisters:
                    byteCount = pdu[1];    // Data Byte Count
                    data = ByteConverter.ToArray(pdu, 2, byteCount);
                    break;
                case FunctionCode.ReadExtendedRegisters:
                    // byteCount = pdu[1]; // Data Byte Count
                    byte groupByteCount = pdu[2]; // Group Data Byte Count
                    // byte referenceType = pdu[3]; // The reference type: 1 byte (must be specified as 6)
                    data = ByteConverter.ToArray(pdu, 4, groupByteCount - 1);
                    break;
                case FunctionCode.WriteSingleCoil:
                case FunctionCode.WriteSingleRegister:
                    data = new byte[2];
                    Array.Copy(pdu, 3, data, 0, 2);
                    break;
                case FunctionCode.WriteMultipleCoils:
                case FunctionCode.WriteMultipleRegisters:
                case FunctionCode.WriteExtendedRegisters:
                    break;
                default:
                    return false;
            }

            return true;
        }

        private static void ValidateData<T>(string argumentName, T[] data, int maxDataLength)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0 || data.Length > maxDataLength)
            {
                string msg = $"The length of argument {argumentName} must be between 1 and {maxDataLength} inclusive.";
                throw new ArgumentOutOfRangeException(msg);
            }
        }

        private static void ValidateNumberOfPoints(string argumentName, ushort numberOfPoints, ushort maxNumberOfPoints)
        {
            if (numberOfPoints < 1 || numberOfPoints > maxNumberOfPoints)
            {
                string msg = $"Argument {argumentName} must be between 1 and {maxNumberOfPoints} inclusive.";
                throw new ArgumentOutOfRangeException(msg);
            }
        }
    }
}
