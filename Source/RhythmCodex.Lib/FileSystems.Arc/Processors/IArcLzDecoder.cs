using System;
using System.IO;

namespace RhythmCodex.FileSystems.Arc.Processors;

public interface IArcLzDecoder
{
    Memory<byte> Decode(Stream source);
}