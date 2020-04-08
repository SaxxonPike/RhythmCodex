using System;

namespace RhythmCodex.Heuristics
{
    public class MemoryHeuristicReader : IHeuristicReader
    {
        private readonly Memory<byte> _bytes;
        private int _offset;

        public MemoryHeuristicReader(Memory<byte> bytes)
        {
            _bytes = bytes;
        }

        public long? Position
        {
            get => _offset;
            set => _offset = (int) (value ?? throw new InvalidOperationException("Can't set position to null."));
        }

        public long? Length => _bytes.Length;

        public ReadOnlySpan<byte> Read(int count)
        {
            var actualLength = Math.Min(_bytes.Length - _offset, count);
            var result = _bytes.Span.Slice(_offset, count);
            _offset += actualLength;
            return result;
        }

        public int Read(byte[] buffer, int offset, int length)
        {
            var actualLength = Math.Min(_bytes.Length - _offset, length);
            _bytes.Span.Slice(_offset, actualLength).CopyTo(buffer.AsSpan(offset));
            _offset += actualLength;
            return actualLength;
        }

        public byte ReadByte()
        {
            return _bytes.Span[_offset++];
        }

        public int ReadInt()
        {
            var span = _bytes.Span.Slice(_offset);
            _offset += 4;
            return span[0] | (span[1] << 8) | (span[2] << 16) | (span[3] << 24);
        }

        public void Skip(int count)
        {
            _offset += count;
        }
    }
}