using System.IO;

namespace RhythmCodex.Infrastructure;

public class BitReader(Stream stream)
{
    private uint _buffer;
    private int _bits;

    public int Peek(int numBits)
    {
        if (numBits == 0)
            return 0;

        // fetch data if we need more
        if (numBits > _bits)
        {
            while (_bits <= 24)
            {
                var b = stream.ReadByte();
                if (b >= 0)
                    _buffer |= (uint) b << (24 - _bits);
                _bits += 8;
            }
        }

        // return the data
        return unchecked((int) (_buffer >> (32 - numBits)));
    }

    public void Remove(int numBits)
    {
        _buffer <<= numBits;
        _bits -= numBits;
    }

    public int Read(int numBits)
    {
        var result = Peek(numBits);
        Remove(numBits);
        return result;
    }
}