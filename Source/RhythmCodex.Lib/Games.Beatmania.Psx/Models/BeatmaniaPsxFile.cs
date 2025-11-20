using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Beatmania.Psx.Models;

[Model]
public class BeatmaniaPsxFile
{
    public Memory<byte> Data { get; set; }
}