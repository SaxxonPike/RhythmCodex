using System.IO;

namespace RhythmCodex.Infrastructure
{
    /// <summary>
    /// Base class for all stream wrappers. Passes through all required Stream functionality to a base stream.
    /// </summary>
    public abstract class StreamWrapper : Stream
    {
        private readonly Stream _baseStream;

        protected StreamWrapper(Stream baseStream)
        {
            _baseStream = baseStream;
        }

        public override void Flush() => _baseStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _baseStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _baseStream.Seek(offset, origin);
        public override void SetLength(long value) => _baseStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _baseStream.Write(buffer, offset, count);
        public override bool CanRead => _baseStream.CanRead;
        public override bool CanSeek => _baseStream.CanSeek;
        public override bool CanWrite => _baseStream.CanWrite;
        public override long Length => _baseStream.Length;

        public override long Position
        {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
        }

        protected override void Dispose(bool disposing) => _baseStream.Dispose();
    }
}