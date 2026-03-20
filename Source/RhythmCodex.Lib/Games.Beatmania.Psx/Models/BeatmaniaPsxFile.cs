using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Beatmania.Psx.Models;

[Model]
public class BeatmaniaPsxFile
{
    public int Index { get; set; }
    public Memory<byte> Data { get; set; }
}