using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions;

[PublicAPI]
[DebuggerStepThrough]
internal static class BinaryWriterExtensions
{
    extension(BinaryWriter writer)
    {
        public void Skip(long offset) => 
            writer.BaseStream.Skip(offset);

        public void Write24(int value)
        {
            Span<byte> buffer = stackalloc byte[4];
            WriteInt32LittleEndian(buffer, value);
            writer.Write(buffer[..3]);
        }

        public void Write24(uint value)
        {
            Span<byte> buffer = stackalloc byte[4];
            WriteUInt32LittleEndian(buffer, value);
            writer.Write(buffer[..3]);
        }

        public void Write24S(int value)
        {
            Span<byte> buffer = stackalloc byte[4];
            WriteInt32BigEndian(buffer, value);
            writer.Write(buffer[1..]);
        }

        public void Write24S(uint value)
        {
            Span<byte> buffer = stackalloc byte[4];
            WriteUInt32BigEndian(buffer, value);
            writer.Write(buffer[1..]);
        }

        public void WriteS(short value)
        {
            Span<byte> buffer = stackalloc byte[2];
            WriteInt16BigEndian(buffer, value);
            writer.Write(buffer);
        }

        public void WriteS(int value)
        {
            Span<byte> buffer = stackalloc byte[4];
            WriteInt32BigEndian(buffer, value);
            writer.Write(buffer);
        }

        public void WriteS(long value)
        {
            Span<byte> buffer = stackalloc byte[8];
            WriteInt64BigEndian(buffer, value);
            writer.Write(buffer);
        }

        public void WriteS(ushort value)
        {
            Span<byte> buffer = stackalloc byte[2];
            WriteUInt16BigEndian(buffer, value);
            writer.Write(buffer);
        }

        public void WriteS(uint value)
        {
            Span<byte> buffer = stackalloc byte[4];
            WriteUInt32BigEndian(buffer, value);
            writer.Write(buffer);
        }

        public void WriteS(ulong value)
        {
            Span<byte> buffer = stackalloc byte[8];
            WriteUInt64BigEndian(buffer, value);
            writer.Write(buffer);
        }

        public void WriteS(ReadOnlySpan<byte> value)
        {
            for (var i = value.Length - 1; i >= 0; i--)
                writer.Write(value[i]);
        }
    }
}