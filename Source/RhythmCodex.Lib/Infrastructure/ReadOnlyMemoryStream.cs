using System;
using System.IO;

namespace RhythmCodex.Infrastructure;

public sealed class ReadOnlyMemoryStream(ReadOnlyMemory<byte> data) : Stream
{
    private int _position;

    public override void Flush()
    {
    }

    public override int Read(Span<byte> buffer)
    {
        switch (buffer.Length)
        {
            case 1:
                if (_position >= data.Length)
                    return 0;
                buffer[0] = data.Span[_position];
                _position++;
                return 1;
            case 0:
                return 0;
            default:
                var length = Math.Min(buffer.Length, data.Length - _position);
                data.Span.Slice(_position, length).CopyTo(buffer);
                _position += length;
                return length;
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        switch (count)
        {
            case 1:
                if (_position >= data.Length)
                    return 0;
                buffer[offset] = data.Span[_position];
                _position++;
                return 1;
            case 0:
                return 0;
            default:
                var length = Math.Min(count, data.Length - _position);
                data.Span.Slice(_position, length).CopyTo(buffer);
                _position += length;
                return length;
        }
    }

    public override void CopyTo(Stream destination, int bufferSize)
    {
        var remaining = data.Span.Length - _position;

        while (remaining > 0)
        {
            var buffer = data.Span.Slice(_position, Math.Min(bufferSize, remaining));
            destination.Write(buffer);
            remaining -= buffer.Length;
            _position += buffer.Length;
        }
    }

    public override int ReadByte()
    {
        if (_position >= data.Length)
            return -1;
        return data.Span[_position++];
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Current:
                _position = (int) (offset + _position);
                break;
            case SeekOrigin.End:
                Position = (int) (Length - offset);
                break;
            default:
                Position = (int) offset;
                break;
        }

        return _position;
    }

    public override void SetLength(long value)
    {
        throw new InvalidOperationException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new InvalidOperationException();
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length => data.Length;

    public override long Position
    {
        get => _position;
        set => _position = (int) value;
    }
}