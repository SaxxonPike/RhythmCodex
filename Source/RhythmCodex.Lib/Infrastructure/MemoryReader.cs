using System;

namespace RhythmCodex.Infrastructure;

public class MemoryReader
{
    private readonly ReadOnlyMemory<byte> _mem;

    public MemoryReader(ReadOnlyMemory<byte> mem)
    {
        _mem = mem;
    }
        
    public int Position { get; set; }

    public byte ReadByte()
    {
        return _mem.Span[Position++];
    }

    public byte[] ReadBytes(int length)
    {
        var result = _mem.Span.Slice(Position, length);
        Position += length;
        return result.ToArray();
    }

    public short ReadInt16()
    {
        var span = _mem.Span.Slice(Position);
        Position += 2;
        return unchecked((short) (span[0] | (span[1] << 8)));
    }
        
    public int ReadInt32()
    {
        var span = _mem.Span.Slice(Position);
        Position += 4;
        return span[0] | (span[1] << 8) | (span[2] << 16) | (span[3] << 24);
    }
}