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
        private IEnumerator<ICdSector> _sectorEnumerator;
        private IsoSectorInfo _currentSector;
        private long _remaining;
        private readonly long? _length;
        private bool _raw;
        private IEnumerable<ICdSector> _sectors;

        public IsoSectorStream(
            IEnumerable<ICdSector> sectors,
            IIsoSectorInfoDecoder isoSectorInfoDecoder,
            long? length,
            bool raw)
        {
            _isoSectorInfoDecoder = isoSectorInfoDecoder;
            _sectors = sectors;
            _length = length;
            _raw = raw;

            Reset();
        }

        private void Reset()
        {
            _position = 0;
            _sectorEnumerator = _sectors.GetEnumerator();
            _sectorEnumerator.MoveNext();

            _currentSector = _isoSectorInfoDecoder
                .Decode(_sectorEnumerator.Current);

            _remaining = _length ?? long.MaxValue;
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

        public override int ReadByte()
        {
            var sector = _raw ? _currentSector.Data.Span : _currentSector.UserData.Span;
            var result = sector[_offset++];
            _position++;

            if (_offset < sector.Length)
                return result;

            _offset -= sector.Length;
            _sectorEnumerator.MoveNext();
            _currentSector = _isoSectorInfoDecoder.Decode(_sectorEnumerator.Current);

            return result;
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

        public override long Seek(long offset, SeekOrigin origin)
        {
            var target = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => _position + offset,
                SeekOrigin.End => throw new NotSupportedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
            };

            //
            // First, see if the target is within the current sector.
            //

            var sectorTarget = target - _position + _offset;
            if (sectorTarget >= 0 && sectorTarget < _currentSector.UserData.Length)
            {
                _offset = (int)sectorTarget;
                _position = target;
                return _position;
            }

            //
            // If the target is ahead in the stream, skip the difference.
            //

            if (target >= _position)
            {
                Skip(target - _position);
                return _position;
            }

            //
            // Rewinding is not possible, so we have to reset the enumerator.
            //

            Reset();
            Skip(target);
            return _position;
        }

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