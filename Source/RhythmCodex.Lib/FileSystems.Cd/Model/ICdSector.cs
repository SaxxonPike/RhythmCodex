using System;

namespace RhythmCodex.FileSystems.Cd.Model;

public interface ICdSector
{
    int Number { get; }
    Memory<byte> Data { get; }
}