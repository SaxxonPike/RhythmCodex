using System;
using System.IO;

namespace RhythmCodex.Infrastructure
{
    public class BitReader
    {
        private readonly Stream _stream;
        private int _bits;
        private ulong _buffer;

        public BitReader(Stream stream)
        {
            _stream = stream;
        }

        public void UnRead(int value, int numBits)
        {
            for (var i = 0; i < numBits; i++)
            {
                var bit = 1UL << _bits;
                _buffer &= ~bit;
                if (((value >> i) & 0x1) != 0)
                    _buffer |= bit;
                _bits++;
            }
        }

        public int Read(int numBits)
        {
            if (numBits == 0)
                return 0;

            if (numBits > _bits)
            {
                while (_bits <= 24)
                {
                    var b = _stream.ReadByte();
                    if (b >= 0)
                        _buffer |= unchecked((uint)(b << (24 - _bits)));

                    _bits += 8;
                }
            }

            var result = _buffer >> (32 - numBits);
            _buffer <<= numBits;
            _bits -= numBits;
            return unchecked((int) result);
        }
    }
}