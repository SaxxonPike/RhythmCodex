using System;
using System.IO;

namespace RhythmCodex.Compression
{
    public interface IBemaniLzss2Decoder
    {
        Memory<byte> DecompressFirebeat(Stream source, int length, int decompLength);
        Memory<byte> DecompressGcz(Stream source, int length, int decompLength);
    }
}