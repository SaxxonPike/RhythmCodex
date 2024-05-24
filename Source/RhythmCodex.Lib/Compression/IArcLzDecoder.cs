using System;
using System.IO;

namespace RhythmCodex.Compression;

public interface IArcLzDecoder
{
    Memory<byte> Decode(Stream source);
}