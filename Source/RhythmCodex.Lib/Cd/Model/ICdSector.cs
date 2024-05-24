using System;

namespace RhythmCodex.Cd.Model;

public interface ICdSector
{
    int Number { get; }
    Memory<byte> Data { get; }
}