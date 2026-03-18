using System;
using RhythmCodex.FileSystems.Iso.Processors;

namespace RhythmCodex.FileSystems.Iso.Helpers;

public static class IsoSectorConverterExtensions
{
    public static byte[] ConvertCookedToRawSector(
        this IIsoSectorConverter converter,
        int frameNumber,
        ReadOnlySpan<byte> sector) =>
        converter.ConvertCookedToRawSector(
            frameNumber / 4500,
            frameNumber / 75 % 60,
            frameNumber % 75, 1,
            sector
        );
}