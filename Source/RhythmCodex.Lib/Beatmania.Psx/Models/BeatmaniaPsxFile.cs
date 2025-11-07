using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Psx.Models;

[Model]
public class BeatmaniaPsxFile
{
    public Memory<byte> Data { get; set; }
}