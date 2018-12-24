using System;
using System.IO;

namespace RhythmCodex.Infrastructure
{
    public class BinaryReaderEx : BinaryReader
    {
        public BinaryReaderEx(Stream source)
            : base(source)
        {
        }

        private int _bitsLeft;
        private int _currentValue;

        public ulong ReadBits(int count)
        {
            ulong result = 0;

            while (count > 0)
            {
                count--;
                if (_bitsLeft <= 0)
                {
                    _bitsLeft = 8;
                    _currentValue = ReadByte();
                }
                _bitsLeft--;
                result <<= 1;
                result |= ((_currentValue & 1) != 0) ? 1UL : 0UL;
            }

            return result;
        }

        public byte[] ReadBytesS(int count)
        {
            var input = ReadBytes(count);
            var result = new byte[count];
            for (int i = 0, j = count - 1; i < count; i++)
                result[i] = input[j--];
            _bitsLeft = 0;
            return result;
        }

        public short ReadInt16S()
        {
            var input = ReadBytes(2);
            short result = input[0];
            result <<= 8;
            result |= (short)input[1];
            _bitsLeft = 0;
            return result;
        }

        public int ReadInt24()
        {
            var input = ReadBytes(3);
            int result = input[2];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[0];
            _bitsLeft = 0;
            return result;
        }

        public int ReadInt24S()
        {
            var input = ReadBytes(3);
            int result = input[0];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[2];
            _bitsLeft = 0;
            return result;
        }

        public int ReadInt32S()
        {
            var input = ReadBytes(4);
            int result = input[0];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[2];
            result <<= 8;
            result |= input[3];
            _bitsLeft = 0;
            return result;
        }

        public long ReadInt64S()
        {
            var input = ReadBytes(8);
            long result = input[0];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[2];
            result <<= 8;
            result |= input[3];
            result <<= 8;
            result |= input[4];
            result <<= 8;
            result |= input[5];
            result <<= 8;
            result |= input[6];
            result <<= 8;
            result |= input[7];
            _bitsLeft = 0;
            return result;
        }

        public byte[] ReadMD5()
        {
            _bitsLeft = 0;
            return ReadBytes(16);
        }

        public byte[] ReadMD5S()
        {
            _bitsLeft = 0;
            return ReadBytesS(16);
        }

        public byte[] ReadSHA1()
        {
            _bitsLeft = 0;
            return ReadBytes(20);
        }

        public byte[] ReadSHA1S()
        {
            _bitsLeft = 0;
            return ReadBytesS(20);
        }

        public long ReadValue(int bytes)
        {
            var buffer = ReadBytes(bytes);
            long result = 0;

            while (bytes > 0)
            {
                bytes--;
                result <<= 8;
                result |= buffer[bytes];
            }

            _bitsLeft = 0;
            return result;
        }

        public long ReadValueS(int bytes)
        {
            var buffer = ReadBytesS(bytes);
            long result = 0;

            while (bytes > 0)
            {
                bytes--;
                result <<= 8;
                result |= buffer[bytes];
            }

            _bitsLeft = 0;
            return result;
        }

        public ushort ReadUInt16S()
        {
            var input = ReadBytes(2);
            ushort result = input[0];
            result <<= 8;
            result |= input[1];
            _bitsLeft = 0;
            return result;
        }

        public uint ReadUInt24()
        {
            var input = ReadBytes(3);
            uint result = input[2];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[0];
            _bitsLeft = 0;
            return result;
        }

        public uint ReadUInt24S()
        {
            var input = ReadBytes(3);
            uint result = input[0];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[2];
            _bitsLeft = 0;
            return result;
        }

        public uint ReadUInt32S()
        {
            var input = ReadBytes(4);
            uint result = input[0];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[2];
            result <<= 8;
            result |= input[3];
            _bitsLeft = 0;
            return result;
        }

        public ulong ReadUInt64S()
        {
            var input = ReadBytes(8);
            ulong result = input[0];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[2];
            result <<= 8;
            result |= input[3];
            result <<= 8;
            result |= input[4];
            result <<= 8;
            result |= input[5];
            result <<= 8;
            result |= input[6];
            result <<= 8;
            result |= input[7];
            _bitsLeft = 0;
            return result;
        }

        public ulong ReadUValue(int bytes)
        {
            var buffer = ReadBytes(bytes);
            ulong result = 0;

            while (bytes > 0)
            {
                bytes--;
                result <<= 8;
                result |= buffer[bytes];
            }

            _bitsLeft = 0;
            return result;
        }

        public ulong ReadUValueS(int bytes)
        {
            var buffer = ReadBytesS(bytes);
            ulong result = 0;

            while (bytes > 0)
            {
                bytes--;
                result <<= 8;
                result |= buffer[bytes];
            }

            _bitsLeft = 0;
            return result;
        }

    }
}