using System;

namespace RhythmCodex.Sounds.Riff.Models;

public class RiffChunk
{
    public string? Id { get; set; }
    public Memory<byte> Data { get; set; }
}