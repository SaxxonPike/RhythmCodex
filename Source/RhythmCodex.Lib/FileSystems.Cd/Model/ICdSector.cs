using System;

namespace RhythmCodex.FileSystems.Cd.Model;

public interface ICdSector
{
    int Number { get; }
    ReadOnlyMemory<byte> Data { get; }
}