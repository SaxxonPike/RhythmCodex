using System;
using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Ddr.S573.Models;

[Model]
public class Ddr573Image
{
    public Dictionary<int, Memory<byte>> Modules { get; set; } = new();
}