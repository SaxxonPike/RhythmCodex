using System;
using System.IO;

namespace RhythmCodex.Ddr.Streamers
{
    public interface IDdr573AudioStreamReader
    {
        byte[] Read(Stream stream, long length, ReadOnlySpan<byte> key, ReadOnlySpan<byte> scramble, int counter);
    }
}