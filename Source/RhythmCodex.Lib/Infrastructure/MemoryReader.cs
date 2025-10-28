using System;

namespace RhythmCodex.Infrastructure;

public class MemoryReader(ReadOnlyMemory<byte> mem)
{
    public int Position { get; set; }

    public byte ReadByte()
    {
        return mem.Span[Position++];
    }

    public byte[] ReadBytes(int length)
    {
        var result = mem.Span.Slice(Position, length);
        Position += length;
        return result.ToArray();
    }

    public short ReadInt16()
    {
        var span = mem.Span[Position..];
        Position += 2;
        return unchecked((short) (span[0] | (span[1] << 8)));
    }
        
    public int ReadInt32()
    {
        var span = mem.Span[Position..];
        Position += 4;
        return span[0] | (span[1] << 8) | (span[2] << 16) | (span[3] << 24);
    }
}