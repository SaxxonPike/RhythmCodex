using System;

namespace RhythmCodex.FileSystems.Cd.Model;

public class CdSector : ICdSector
{
    public int Number { get; set; }
    public Memory<byte> Data { get; set; }
}