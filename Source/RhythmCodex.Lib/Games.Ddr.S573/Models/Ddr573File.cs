using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Ddr.S573.Models;

[Model]
public class Ddr573File
{
    public int Id { get; set; }
    public int Offset { get; set; }
    public int Module { get; set; }
    public int EncryptionType { get; set; }
    public int Reserved1 { get; set; }
    public int Reserved2 { get; set; }
    public Memory<byte> Data { get; set; }
}