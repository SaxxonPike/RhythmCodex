using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Cd.Streamers;

[NotService]
public class CdSectorStream(IReadOnlyList<ICdSector> sectors) : Stream
{
    private int _sectorOffset;
    private int _sectorIndex;

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var remaining = count;
        var result = 0;
        while (remaining > 0)
        {
            var data = sectors[_sectorIndex].Data;
            if (remaining > data.Length - _sectorIndex)
            {
                data.Span[_sectorIndex..].CopyTo(buffer.AsSpan(offset));
                remaining -= data.Length - _sectorIndex;
                result += data.Length - _sectorIndex;
                _sectorIndex = 0;
            }
            else
            {
                data.Span.Slice(_sectorIndex, remaining).CopyTo(buffer.AsSpan(offset));
                _sectorIndex += remaining;
                result += remaining;
                remaining = 0;
            }
        }

        return result;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
            {
                _sectorOffset = (int) (offset / 0x800);
                _sectorIndex = (int) (offset % 0x800);
                return offset;
            }
            case SeekOrigin.Current:
            {
                var newOffset = _sectorOffset * 0x800L + _sectorIndex + offset;
                _sectorOffset = (int) (newOffset / 0x800);
                _sectorIndex = (int) (newOffset % 0x800);
                return newOffset;
            }
            default:
            {
                throw new NotImplementedException(
                    $"SeekOrigin other than {nameof(SeekOrigin.Begin)} or {nameof(SeekOrigin.Current)} " +
                    "is not supported.");
            }
        }
    }

    public override void SetLength(long value)
    {
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException($"Writing to {nameof(CdSectorStream)} is not supported.");
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => sectors.Count * 0x800L;

    public override long Position
    {
        get => _sectorOffset * 0x800L + _sectorIndex;
        set => Seek(value, SeekOrigin.Begin);
    }
}