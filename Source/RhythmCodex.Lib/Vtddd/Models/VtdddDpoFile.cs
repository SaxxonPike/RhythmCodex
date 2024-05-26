using System;

namespace RhythmCodex.Vtddd.Models;

public class VtdddDpoFile
{
    public Memory<byte> Key { get; set; }
    public Memory<byte> Data { get; set; }
}