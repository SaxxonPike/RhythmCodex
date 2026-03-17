using System;

namespace RhythmCodex.FileSystems.Iso.Processors;

public interface IIsoSectorExpander
{
    byte[] Expand2048To2352(int minute, int second, int frame, int mode, ReadOnlySpan<byte> sector);
}