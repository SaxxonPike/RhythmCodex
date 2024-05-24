using System;
using System.IO;

namespace RhythmCodex.Compression;

public interface IBemaniLzssDecoder
{
    Memory<byte> Decode(Stream source);
}