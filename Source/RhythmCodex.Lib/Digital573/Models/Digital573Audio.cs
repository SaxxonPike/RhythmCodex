using System;

namespace RhythmCodex.Digital573.Models;

public class Digital573Audio
{
    public Memory<byte> Data { get; set; }
    public Memory<byte> Key { get; set; }
    public int Counter { get; set; }
}