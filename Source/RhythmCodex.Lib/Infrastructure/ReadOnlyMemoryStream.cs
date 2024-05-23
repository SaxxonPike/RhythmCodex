using System;
using System.IO;

namespace RhythmCodex.Infrastructure;

public sealed class ReadOnlyMemoryStream : Stream
{
    private readonly ReadOnlyMemory<byte> _data;
    private int _position;

    public ReadOnlyMemoryStream(ReadOnlyMemory<byte> data)
    {
        _data = data;
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        switch (count)
        {
            case 1:
                if (_position >= _data.Length)
                    return 0;
                buffer[offset] = _data.Span[_position];
                _position++;
                return 1;
            case 0:
                return 0;
            default:
                var length = Math.Min(count, _data.Length - _position);
                _data.Span.Slice(_position, length).CopyTo(buffer);
                _position += length;
                return length;
        }
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

    public override long Length => _data.Length;

    public override long Position
    {
        get => _position;
        set => _position = (int) value;
    }
}