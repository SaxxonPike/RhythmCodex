using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Xact.Model;

[Model]
public class XwbSound
{
    public Memory<byte> Data { get; set; }
    public string? Name { get; set; }
    public XwbEntry Info { get; set; }
}