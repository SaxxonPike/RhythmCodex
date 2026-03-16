using System;
using System.Collections.Generic;
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
        private int _offset;
        private long _position;
        private readonly IEnumerator<ICdSector> _sectorEnumerator;
        private IsoSectorInfo _currentSector;
        private long _remaining;
        private readonly long? _length;
        private bool _raw;

        public IsoSectorStream(
            IEnumerable<ICdSector> sectors,
            IIsoSectorInfoDecoder isoSectorInfoDecoder,
            long? length,
            bool raw)
        {
            _isoSectorInfoDecoder = isoSectorInfoDecoder;
            _sectorEnumerator = sectors.GetEnumerator();
            _sectorEnumerator.MoveNext();

            _currentSector = _isoSectorInfoDecoder
                .Decode(_sectorEnumerator.Current);

            _remaining = length ?? long.MaxValue;
            _length = length;
            _raw = raw;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count) =>
            Read(buffer.AsSpan(offset, count));

        public override int Read(Span<byte> buffer)
        {
            var offset = 0;
            var count = buffer.Length;
            var remaining = Math.Min(count, _remaining);
            var result = 0;
            var sector = _raw ? _currentSector.Data.Span : _currentSector.UserData.Span;

            while (remaining > 0)
            {
                buffer[offset] = sector[_offset];
                offset++;
                _offset++;
                _position++;
                if (_offset >= sector.Length)
                {
                    _offset -= sector.Length;
                    _sectorEnumerator.MoveNext();
                    _currentSector = _isoSectorInfoDecoder.Decode(_sectorEnumerator.Current);
                    sector = _raw ? _currentSector.Data.Span : _currentSector.UserData.Span;
                }

                remaining--;
                _remaining--;
                result++;
            }

            return result;
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
        public override void SetLength(long value) => throw new NotImplementedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _length ?? throw new NotSupportedException();

        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        protected override void Dispose(bool disposing)
        {
            _sectorEnumerator.Dispose();
            base.Dispose(disposing);
        }
    }

    public Stream Open(IEnumerable<ICdSector> sectors)
    {
        return new IsoSectorStream(sectors, isoSectorInfoDecoder, null, false);
    }

    public Stream Open(IEnumerable<ICdSector> sectors, long length)
    {
        return new IsoSectorStream(sectors, isoSectorInfoDecoder, length, false);
    }

    public Stream OpenRaw(IEnumerable<ICdSector> sectors)
    {
        return new IsoSectorStream(sectors, isoSectorInfoDecoder, null, true);
    }

    public Stream OpenRaw(IEnumerable<ICdSector> sectors, long length)
    {
        return new IsoSectorStream(sectors, isoSectorInfoDecoder, length, true);
    }
}