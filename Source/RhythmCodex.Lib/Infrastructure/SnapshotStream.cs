using System;
using System.IO;

namespace RhythmCodex.Infrastructure
{
    /// <summary>
    /// Wraps a stream that spies on reads, writing the data to an internal buffer.
    /// Handy for when you need to know how big something is, but can't know unless you read it all.
    /// </summary>
    public class SnapshotStream : StreamWrapper
    {
        private readonly MemoryStream _buffer;

        public SnapshotStream(Stream baseStream) : base(baseStream)
        {
            _buffer = new MemoryStream();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = base.Read(buffer, offset, count);
            var data = buffer.AsSpan(offset, result).ToArray();
            _buffer.Write(data, 0, result);
            return result;
        }

        public byte[] ToArray() => _buffer.ToArray();
        public ReadOnlySpan<byte> ToSpan() => _buffer.GetBuffer();

        public void Reset()
        {
            _buffer.Position = 0;
            _buffer.SetLength(0);
        }
    }
}