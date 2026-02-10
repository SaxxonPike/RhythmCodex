using System.Diagnostics;
using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions;

[DebuggerStepThrough]
internal static class BinaryReaderExtensions
{
    extension(BinaryReader reader)
    {
        public void Skip(long offset) => 
            reader.BaseStream.Skip(offset);

        public byte[] ReadBytesS(int count)
        {
            var input = reader.ReadBytes(count);
            var result = new byte[count];
            for (int i = 0, j = count - 1; i < count; i++)
                result[i] = input[j--];
            return result;
        }

        public short ReadInt16S()
        {
            var input = reader.ReadBytes(2);
            int result = input[0];
            result <<= 8;
            result |= input[1];
            return unchecked((short) result);
        }

        public int ReadInt24()
        {
            var input = reader.ReadBytes(3);
            int result = input[2];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[0];
            return result;
        }

        public int ReadInt24S()
        {
            var input = reader.ReadBytes(3);
            int result = input[0];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[2];
            return result;
        }

        public int ReadInt32S()
        {
            var input = reader.ReadBytes(4);
            int result = input[0];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[2];
            result <<= 8;
            result |= input[3];
            return result;
        }

        public long ReadInt64S()
        {
            var input = reader.ReadBytes(8);
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
            return result;
        }

        public byte[] ReadMD5()
        {
            return reader.ReadBytes(16);
        }

        public byte[] ReadMD5S()
        {
            return reader.ReadBytesS(16);
        }

        public byte[] ReadSHA1()
        {
            return reader.ReadBytes(20);
        }

        public byte[] ReadSHA1S()
        {
            return reader.ReadBytesS(20);
        }

        public long ReadValue(int bytes)
        {
            var buffer = reader.ReadBytes(bytes);
            long result = 0;

            while (bytes > 0)
            {
                bytes--;
                result <<= 8;
                result |= buffer[bytes];
            }

            return result;
        }

        public long ReadValueS(int bytes)
        {
            var buffer = reader.ReadBytesS(bytes);
            long result = 0;

            while (bytes > 0)
            {
                bytes--;
                result <<= 8;
                result |= buffer[bytes];
            }

            return result;
        }

        public ushort ReadUInt16S()
        {
            var input = reader.ReadBytes(2);
            ushort result = input[0];
            result <<= 8;
            result |= input[1];

            return result;
        }

        public uint ReadUInt24()
        {
            var input = reader.ReadBytes(3);
            uint result = input[2];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[0];

            return result;
        }

        public uint ReadUInt24S()
        {
            var input = reader.ReadBytes(3);
            uint result = input[0];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[2];

            return result;
        }

        public uint ReadUInt32S()
        {
            var input = reader.ReadBytes(4);
            uint result = input[0];
            result <<= 8;
            result |= input[1];
            result <<= 8;
            result |= input[2];
            result <<= 8;
            result |= input[3];

            return result;
        }

        public ulong ReadUInt64S()
        {
            var input = reader.ReadBytes(8);
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

            return result;
        }

        public ulong ReadUValue(int bytes)
        {
            var buffer = reader.ReadBytes(bytes);
            ulong result = 0;

            while (bytes > 0)
            {
                bytes--;
                result <<= 8;
                result |= buffer[bytes];
            }

            return result;
        }

        public ulong ReadUValueS(int bytes)
        {
            var buffer = reader.ReadBytesS(bytes);
            ulong result = 0;

            while (bytes > 0)
            {
                bytes--;
                result <<= 8;
                result |= buffer[bytes];
            }

            return result;
        }
    }
}