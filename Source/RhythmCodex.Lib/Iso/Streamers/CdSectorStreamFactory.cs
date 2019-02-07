using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Converters;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Streamers
{
    [Service]
    public class CdSectorStreamFactory : ICdSectorStreamFactory
    {
        private readonly ICdSectorInfoDecoder _cdSectorInfoDecoder;

        private sealed class CdSectorStream : Stream
        {
            private readonly int _sectorSize;
            private readonly ICdSectorInfoDecoder _cdSectorInfoDecoder;
            private int _offset = 0;
            private int _sector = 0;
            private readonly IEnumerator<ICdSector> _sectorEnumerator;

            public CdSectorStream(int sectorSize, IEnumerable<ICdSector> sectors, ICdSectorInfoDecoder cdSectorInfoDecoder)
            {
                _sectorSize = sectorSize;
                _cdSectorInfoDecoder = cdSectorInfoDecoder;
                _sectorEnumerator = sectors.GetEnumerator();
                _sectorEnumerator.MoveNext();
            }
            
            public override void Flush()
            {
                throw new NotImplementedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var remaining = count;
                var sector = _cdSectorInfoDecoder.Decode(_sectorEnumerator.Current).Data;
                var result = 0;
            
                while (remaining > 0)
                {
                    buffer[offset] = sector[_offset];
                    offset++;
                    _offset++;
                    if (_offset > _sectorSize)
                    {
                        _offset -= _sectorSize;
                        _sector++;
                        _sectorEnumerator.MoveNext();
                        sector = _cdSectorInfoDecoder.Decode(_sectorEnumerator.Current).Data;
                    }
                    
                    remaining--;
                    result++;
                }

                return result;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                    {
                        _sector = (int) (offset / _sectorSize);
                        _offset = (int) (offset % _sectorSize);
                        return offset;
                    }
                    case SeekOrigin.Current:
                    {
                        var newOffset = offset + _sector * _sectorSize + _offset;
                        _sector = (int) (newOffset / _sectorSize);
                        _offset = (int) (newOffset % _sectorSize);
                        return newOffset;
                    }
                    default:
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => _sector * _sectorSize + _offset;
                set => Seek(value, SeekOrigin.Begin);
            }

            protected override void Dispose(bool disposing)
            {
                _sectorEnumerator.Dispose();
                base.Dispose(disposing);
            }
        }
        
        public CdSectorStreamFactory(ICdSectorInfoDecoder cdSectorInfoDecoder)
        {
            _cdSectorInfoDecoder = cdSectorInfoDecoder;
        }

        public Stream Open(IEnumerable<ICdSector> sectors)
        {
            return new CdSectorStream(2048, sectors, _cdSectorInfoDecoder);
        }
    }
}