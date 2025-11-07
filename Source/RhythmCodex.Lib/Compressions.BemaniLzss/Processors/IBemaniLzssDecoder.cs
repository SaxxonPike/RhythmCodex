using System;
using System.IO;

namespace RhythmCodex.Compressions.BemaniLzss.Processors;

public interface IBemaniLzssDecoder
{
    Memory<byte> Decode(Stream source);
}