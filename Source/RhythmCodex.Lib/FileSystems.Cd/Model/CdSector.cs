using System;

namespace RhythmCodex.FileSystems.Cd.Model;

public class CdSector : ICdSector
{
    private static readonly Lazy<ReadOnlyMemory<byte>> EmptyData = new(() => new byte[2352]);

    public static CdSector Empty(int number, int sectorSize) =>
        new() { Number = number, Data = EmptyData.Value[..sectorSize] };

    public int Number { get; init; }
    public ReadOnlyMemory<byte> Data { get; init; }
}