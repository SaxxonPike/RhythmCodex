using System;
using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Heuristics
{
    public class StreamHeuristicReader : IHeuristicReader
    {
        private readonly Stream _stream;
        private readonly BinaryReader _reader;

        public StreamHeuristicReader(Stream stream)
        {
            _stream = stream;
            _reader = new BinaryReader(_stream);
        }

        public long? Position
        {
            get => _stream.Position;
            set => _stream.Position = value ?? throw new InvalidOperationException("Can't set position to null.");
        }

        public long? Length => _stream.Length;

        public ReadOnlySpan<byte> Read(int count)
        {
            var buffer = new byte[count];
            var actualRead = _stream.TryRead(buffer, 0, count);
            return buffer.AsSpan(0, actualRead);
        }

        public int Read(byte[] buffer, int offset, int length) =>
            _stream.Read(buffer, offset, length);

        public byte ReadByte() => _reader.ReadByte();
        public int ReadInt() => _reader.ReadInt32();

        public void Skip(int count)
        {
            if (_stream.CanSeek)
                _stream.Seek(count, SeekOrigin.Current);
            else
                _stream.Skip(count);
        }
    }
}