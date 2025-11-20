using System;
using System.IO;

namespace RhythmCodex.Compressions.BemaniLz.Processors;

public interface IBemaniLzDecoder
{
    Memory<byte> Decode(Stream source);
}