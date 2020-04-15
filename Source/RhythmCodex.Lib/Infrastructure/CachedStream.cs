using System;
using System.IO;

namespace RhythmCodex.Infrastructure
{
    /// <summary>
    /// A "cached" stream. It buffers all data read through this wrapper which can be retrieved again or
    /// seeked through. Useful for forward-only streams where you might need to reposition the stream anyway.
    /// </summary>
    public class CachedStream : StreamWrapper
    {
        private MemoryStream _cache;

        public CachedStream(Stream baseStream)
            : base(baseStream)
        {
            _cache = new MemoryStream();
            Reset();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var remaining = _cache.Length - _cache.Position;

            if (remaining <= 0)
                return DoStreamRead(buffer, offset, count);

            if (remaining >= count)
                return DoCacheRead(buffer, offset, count);

            return DoCacheRead(buffer, offset, (int) remaining) + 
                   DoStreamRead(buffer, (int) (offset + remaining), (int) (count - remaining));
        }

        private int DoStreamRead(byte[] buffer, int offset, int length)
        {
            var bytesRead = BaseStream.TryRead(buffer, offset, length);
            _cache.Write(buffer, offset, bytesRead);
            return bytesRead;
        }

        private int DoCacheRead(byte[] buffer, int offset, int length)
        {
            var bytesRead = _cache.Read(buffer, offset, length);
            return bytesRead;
        }

        public override bool CanSeek => true;

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Current:
                {
                    return Seek(_cache.Position + offset, SeekOrigin.Begin);
                }
                case SeekOrigin.End:
                {
                    throw new NotSupportedException("Can't seek from end - not supported yet.");
                }
                case SeekOrigin.Begin:
                {
                    if (offset < 0)
                        throw new InvalidOperationException("Can't seek to before cache started.");

                    if (offset <= _cache.Length)
                    {
                        _cache.Position = offset;
                        break;
                    }

                    _cache.Position = _cache.Length;
                    this.SkipBytes(offset - _cache.Length);
                    break;
                }
            }

            return _cache.Position;
        }

        public override long Position
        {
            get => _cache.Position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public byte[] ToArray() => _cache.ToArray();

        public void Reset()
        {
            Rewind();
            _cache.SetLength(0);
        }

        public void Rewind()
        {
            _cache.Position = 0;
        }

        /// <summary>
        /// Removes a specified number of bytes from the front of the buffer and adjusts the cache pointer accordingly.
        /// </summary>
        public void Advance(int offset)
        {
            if (offset == 0)
                return;
            
            if (offset < 0)
                throw new InvalidOperationException("Can't advance a negative number of bytes.");
            
            if (offset >= _cache.Length)
            {
                Reset();
                return;
            }

            var newPosition = _cache.Position - offset;
            _cache.Position = offset;
            var newBuffer = new MemoryStream();
            _cache.CopyTo(newBuffer);
            var oldBuffer = _cache;
            _cache = newBuffer;
            oldBuffer.Dispose();

            if (newPosition < 0)
                newPosition = 0;

            _cache.Position = newPosition;
        }
    }
}