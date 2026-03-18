using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.FileSystems.Cd.Model;

public class CdSector : ICdSector
{
    public const int RawSectorSize = 2352;
    public const int CookedSectorSize = 2048;

    public static CdSector Empty(int number, int sectorSize) =>
        new() { Number = number, Data = ZeroMemory.MemoryInstance(sectorSize) };

    public static CdSector FromMemory(int number, ReadOnlyMemory<byte> memory) =>
        new() { Number = number, Data = memory };

    public int Number { get; init; }
    public ReadOnlyMemory<byte> Data { get; init; }
}