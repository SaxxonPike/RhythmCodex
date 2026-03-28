using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace RhythmCodex.Utils.Cursors;

[PublicAPI]
public static class StreamExtensions
{
    extension(Stream stream)
    {
        [DebuggerStepThrough]
        public byte[] Read([NonNegativeValue] int count)
        {
            var data = new byte[count];
            var actual = stream.ReadAtLeast(data, count, false);
            if (actual < data.Length)
                Array.Resize(ref data, actual);
            return data;
        }

        [DebuggerStepThrough]
        public Stream ReadU8(out byte val)
        {
            Span<byte> buffer = stackalloc byte[1];
            stream.ReadExactly(buffer);
            val = buffer[0];
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteU8(byte val)
        {
            Span<byte> buffer = stackalloc byte[1];
            buffer[0] = val;
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadS8(out sbyte val)
        {
            Span<byte> buffer = stackalloc byte[1];
            stream.ReadExactly(buffer);
            val = unchecked((sbyte)buffer[0]);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteS8(sbyte val)
        {
            Span<byte> buffer = stackalloc byte[1];
            buffer[0] = unchecked((byte)val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadU16L(out ushort val)
        {
            Span<byte> buffer = stackalloc byte[2];
            stream.ReadExactly(buffer);
            val = ReadUInt16LittleEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteU16L(ushort val)
        {
            Span<byte> buffer = stackalloc byte[2];
            WriteUInt16LittleEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadU16B(out ushort val)
        {
            Span<byte> buffer = stackalloc byte[2];
            stream.ReadExactly(buffer);
            val = ReadUInt16BigEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteU16B(ushort val)
        {
            Span<byte> buffer = stackalloc byte[2];
            WriteUInt16BigEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadS16L(out short val)
        {
            Span<byte> buffer = stackalloc byte[2];
            stream.ReadExactly(buffer);
            val = ReadInt16LittleEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteS16L(short val)
        {
            Span<byte> buffer = stackalloc byte[2];
            WriteInt16LittleEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadS16B(out short val)
        {
            Span<byte> buffer = stackalloc byte[2];
            stream.ReadExactly(buffer);
            val = ReadInt16BigEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteS16B(short val)
        {
            Span<byte> buffer = stackalloc byte[2];
            WriteInt16BigEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadU32L(out uint val)
        {
            Span<byte> buffer = stackalloc byte[4];
            stream.ReadExactly(buffer);
            val = ReadUInt32LittleEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteU32L(uint val)
        {
            Span<byte> buffer = stackalloc byte[4];
            WriteUInt32LittleEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadU32B(out uint val)
        {
            Span<byte> buffer = stackalloc byte[4];
            stream.ReadExactly(buffer);
            val = ReadUInt32BigEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteU32B(uint val)
        {
            Span<byte> buffer = stackalloc byte[4];
            WriteUInt32BigEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadS32L(out int val)
        {
            Span<byte> buffer = stackalloc byte[4];
            stream.ReadExactly(buffer);
            val = ReadInt32LittleEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteS32L(int val)
        {
            Span<byte> buffer = stackalloc byte[4];
            WriteInt32LittleEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadS32B(out int val)
        {
            Span<byte> buffer = stackalloc byte[4];
            stream.ReadExactly(buffer);
            val = ReadInt32BigEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteS32B(int val)
        {
            Span<byte> buffer = stackalloc byte[4];
            WriteInt32BigEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadF32L(out float val)
        {
            Span<byte> buffer = stackalloc byte[4];
            stream.ReadExactly(buffer);
            val = ReadSingleLittleEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteF32L(float val)
        {
            Span<byte> buffer = stackalloc byte[4];
            WriteSingleLittleEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadF32B(out float val)
        {
            Span<byte> buffer = stackalloc byte[4];
            stream.ReadExactly(buffer);
            val = ReadSingleBigEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteF32B(float val)
        {
            Span<byte> buffer = stackalloc byte[4];
            WriteSingleBigEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadU64L(out ulong val)
        {
            Span<byte> buffer = stackalloc byte[8];
            stream.ReadExactly(buffer);
            val = ReadUInt64LittleEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteU64L(ulong val)
        {
            Span<byte> buffer = stackalloc byte[8];
            WriteUInt64LittleEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadU64B(out ulong val)
        {
            Span<byte> buffer = stackalloc byte[8];
            stream.ReadExactly(buffer);
            val = ReadUInt64BigEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteU64B(ulong val)
        {
            Span<byte> buffer = stackalloc byte[8];
            WriteUInt64BigEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadS64L(out long val)
        {
            Span<byte> buffer = stackalloc byte[8];
            stream.ReadExactly(buffer);
            val = ReadInt64LittleEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteS64L(long val)
        {
            Span<byte> buffer = stackalloc byte[8];
            WriteInt64LittleEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadS64B(out long val)
        {
            Span<byte> buffer = stackalloc byte[8];
            stream.ReadExactly(buffer);
            val = ReadInt64BigEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteS64B(long val)
        {
            Span<byte> buffer = stackalloc byte[8];
            WriteInt64BigEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadF64L(out double val)
        {
            Span<byte> buffer = stackalloc byte[8];
            stream.ReadExactly(buffer);
            val = ReadDoubleLittleEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteF64L(double val)
        {
            Span<byte> buffer = stackalloc byte[8];
            WriteDoubleLittleEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream ReadF64B(out double val)
        {
            Span<byte> buffer = stackalloc byte[8];
            stream.ReadExactly(buffer);
            val = ReadDoubleBigEndian(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream WriteF64B(double val)
        {
            Span<byte> buffer = stackalloc byte[8];
            WriteDoubleBigEndian(buffer, val);
            stream.Write(buffer);
            return stream;
        }

        [DebuggerStepThrough]
        public Stream Skip([NonNegativeValue] int count)
        {
            if (stream.CanSeek)
            {
                stream.Seek(count, SeekOrigin.Current);
                return stream;
            }

            switch (count)
            {
                case 0:
                    return stream;
                case <= 256:
                {
                    Span<byte> buffer = stackalloc byte[count];
                    stream.ReadAtLeast(buffer, count, false);
                    return stream;
                }
                default:
                {
                    using var mem = MemoryPool<byte>.Shared.Rent();
                    var span = mem.Memory.Span;
                    var remaining = count;
                    while (remaining > 0)
                    {
                        var amount = stream.Read(span[..Math.Max(span.Length, remaining)]);
                        if (amount < 1)
                            break;
                        remaining -= amount;
                    }

                    return stream;
                }
            }
        }
    }
}