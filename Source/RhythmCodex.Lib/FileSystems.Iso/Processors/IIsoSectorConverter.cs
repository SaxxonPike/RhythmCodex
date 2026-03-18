using System;

namespace RhythmCodex.FileSystems.Iso.Processors;

public interface IIsoSectorConverter
{
    byte[] ConvertCookedToRawSector(int minute, int second, int frame, int mode, ReadOnlySpan<byte> sector);
}