using System;

namespace RhythmCodex.Cd.Model;

public class CdSector : ICdSector
{
    public int Number { get; set; }
    public Memory<byte> Data { get; set; }
}