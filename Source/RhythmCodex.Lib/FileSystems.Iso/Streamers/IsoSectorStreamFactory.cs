using System;
using System.IO;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Iso.Converters;
using RhythmCodex.FileSystems.Iso.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Iso.Streamers;

[Service]
public class IsoSectorStreamFactory(IIsoSectorInfoDecoder isoSectorInfoDecoder) : IIsoSectorStreamFactory
{
    private sealed class IsoSectorStream : Stream
    {
        private readonly IIsoSectorInfoDecoder _isoSectorInfoDecoder;
        private readonly long _length;
        private readonly bool _raw;
        private readonly ICdSectorCollection _sectors;
        private IsoSectorInfo? _currentSector;
        private long _position;

        public IsoSectorStream(
            ICdSectorCollection sectors,
            IIsoSectorInfoDecoder isoSectorInfoDecoder,
            long? length,
            bool raw)
        {
            _isoSectorInfoDecoder = isoSectorInfoDecoder;
            _sectors = sectors;
            _length = length ?? sectors.Count * (_raw ? CdSector.RawSectorSize : CdSector.CookedSectorSize);
            _raw = raw;
        }

        private void Skip(long offset)
        {
            if (offset <= 0)
                return;

            Span<byte> buffer = stackalloc byte[4096];

            var remaining = (int)offset;

            while (remaining > 0)
            {
                var count = Math.Min(buffer.Length, remaining);
                ReadExactly(buffer[..count]);
                remaining -= count;
            }
        }

        public override void Flush()
        {
        }

        private (IsoSectorInfo? Info, int Offset, ReadOnlyMemory<byte> Data) GetSector()
        {
            var sectorSize = _raw ? CdSector.RawSectorSize : CdSector.CookedSectorSize;
            var sector = (int)_position / sectorSize;

            if (_currentSector == null || sector != _currentSector.Number)
            {
                var nextSector = _sectors[sector];
                _currentSector = _isoSectorInfoDecoder.Decode(nextSector);
            }

            var data = _raw ? _currentSector.Data : _currentSector.UserData;
            return (_currentSector, (int)(_position - sector * sectorSize), data);
        }

        public override int ReadByte()
        {
            var (_, offset, data) = GetSector();
            var result = data.Span[offset];
            _position++;
            return result;
        }

        public override int Read(byte[] buffer, int offset, int count) =>
            Read(buffer.AsSpan(offset, count));

        public override int Read(Span<byte> buffer)
        {
            var count = buffer.Length;
            var remaining = Math.Min(count, _length - _position);
            var result = 0;
            var (sector, inOffset, data) = GetSector();
            var sectorData = data.Span;
            var outOffset = 0;

            while (remaining > 0)
            {
                var src = sectorData[inOffset..];
                var dst = buffer[outOffset..];
                var len = (int)Math.Min(src.Length, remaining);

                src[..len].CopyTo(dst);

                _position += len;
                remaining -= len;
                result += len;
                outOffset += len;

                if (remaining <= 0)
                    break;

                (sector, inOffset, data) = GetSector();
                sectorData = data.Span;
            }

            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            _position = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => _position + offset,
                SeekOrigin.End => _length - offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
            };

            return _position;
        }

        public override void SetLength(long value) => throw new NotImplementedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _length;

        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }
    }

    public Stream Open(ICdSectorCollection sectors)
    {
        return new IsoSectorStream(sectors, isoSectorInfoDecoder, null, false);
    }

    public Stream Open(ICdSectorCollection sectors, long length)
    {
        return new IsoSectorStream(sectors, isoSectorInfoDecoder, length, false);
    }

    public Stream OpenRaw(ICdSectorCollection sectors)
    {
        return new IsoSectorStream(sectors, isoSectorInfoDecoder, null, true);
    }

    public Stream OpenRaw(ICdSectorCollection sectors, long length)
    {
        return new IsoSectorStream(sectors, isoSectorInfoDecoder, length, true);
    }
}