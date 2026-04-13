using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions;

[PublicAPI]
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
            Span<byte> buffer = stackalloc byte[2];
            reader.ReadExactly(buffer);
            return ReadInt16BigEndian(buffer);
        }

        public int ReadInt24()
        {
            Span<byte> buffer = stackalloc byte[4];
            reader.ReadExactly(buffer);
            return (ReadInt32LittleEndian(buffer) << 8) >> 8;
        }

        public int ReadInt24S()
        {
            Span<byte> buffer = stackalloc byte[4];
            reader.ReadExactly(buffer[1..]);
            return (ReadInt32BigEndian(buffer) << 8) >> 8;
        }

        public int ReadInt32S()
        {
            Span<byte> buffer = stackalloc byte[4];
            reader.ReadExactly(buffer);
            return ReadInt32BigEndian(buffer);
        }

        public long ReadInt64S()
        {
            Span<byte> buffer = stackalloc byte[8];
            reader.ReadExactly(buffer);
            return ReadInt64BigEndian(buffer);
        }

        public byte[] ReadMD5() => 
            reader.ReadBytes(16);

        public byte[] ReadMD5S() => 
            reader.ReadBytesS(16);

        public byte[] ReadSHA1() => 
            reader.ReadBytes(20);

        public byte[] ReadSHA1S() => 
            reader.ReadBytesS(20);

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
            Span<byte> buffer = stackalloc byte[2];
            reader.ReadExactly(buffer);
            return ReadUInt16BigEndian(buffer);
        }

        public uint ReadUInt24()
        {
            Span<byte> buffer = stackalloc byte[4];
            reader.ReadExactly(buffer);
            return ReadUInt32LittleEndian(buffer) & 0x00FFFFFFU;
        }

        public uint ReadUInt24S()
        {
            Span<byte> buffer = stackalloc byte[4];
            reader.ReadExactly(buffer[1..]);
            return ReadUInt32BigEndian(buffer) & 0x00FFFFFFU;
        }

        public uint ReadUInt32S()
        {
            Span<byte> buffer = stackalloc byte[4];
            reader.ReadExactly(buffer);
            return ReadUInt32BigEndian(buffer);
        }

        public ulong ReadUInt64S()
        {
            Span<byte> buffer = stackalloc byte[8];
            reader.ReadExactly(buffer);
            return ReadUInt64BigEndian(buffer);
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