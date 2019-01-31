using System;
using System.IO;

namespace RhythmCodex.Compression
{
    public interface IBemaniLzDecoder
    {
        Memory<byte> Decode(Stream source);
    }
}