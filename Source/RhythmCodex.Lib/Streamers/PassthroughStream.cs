using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Streamers
{
    [NotService]
    public abstract class PassthroughStream : Stream
    {
        protected Stream BaseStream { get; }

        internal PassthroughStream(Stream baseStream)
        {
            BaseStream = baseStream;
        }

        public override void Flush() => BaseStream.Flush();
        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);
        public override void SetLength(long value) => BaseStream.SetLength(value);
        public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);
        public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, count);

        public override bool CanRead => BaseStream.CanRead;
        public override bool CanSeek => BaseStream.CanSeek;
        public override bool CanWrite => BaseStream.CanWrite;
        public override long Length => BaseStream.Length;

        public override long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public byte[] ReadToEnd() => this.ReadAllBytes();
    }
}