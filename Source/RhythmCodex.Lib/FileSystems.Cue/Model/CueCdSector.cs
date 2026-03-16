using System;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Cue.Model;

public class CueCdSector : ICdSector
{
    public int Number { get; init; }
    public ReadOnlyMemory<byte> Data { get; init; }
}