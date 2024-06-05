using System;

namespace RhythmCodex.Heuristics;

public class MemoryHeuristicReader(ReadOnlyMemory<byte> bytes) : IHeuristicReader
{
    private int _offset;

    public long? Position
    {
        get => _offset;
        set => _offset = (int) (value ?? throw new InvalidOperationException("Can't set position to null."));
    }

    public long? Length => bytes.Length;

    public ReadOnlySpan<byte> Read(int count)
    {
        var actualLength = Math.Min(bytes.Length - _offset, count);
        var result = bytes.Span.Slice(_offset, actualLength);
        _offset += actualLength;
        return result;
    }

    public int Read(Span<byte> buffer)
    {
        var actualLength = Math.Min(bytes.Length - _offset, buffer.Length);
        var result = bytes.Span.Slice(_offset, actualLength);
        _offset += actualLength;
        result.CopyTo(buffer);
        return actualLength;
    }

    public int Read(byte[] buffer, int offset, int length)
    {
        var actualLength = Math.Min(bytes.Length - _offset, length);
        bytes.Span.Slice(_offset, actualLength).CopyTo(buffer.AsSpan(offset));
        _offset += actualLength;
        return actualLength;
    }

    public byte ReadByte()
    {
        return bytes.Span[_offset++];
    }

    public int ReadInt()
    {
        var span = bytes.Span.Slice(_offset);
        _offset += 4;
        return span[0] | (span[1] << 8) | (span[2] << 16) | (span[3] << 24);
    }

    public void Skip(int count)
    {
        _offset += count;
    }
}