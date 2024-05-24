using System;

namespace RhythmCodex.Riff.Models;

public class RiffChunk : IRiffChunk
{
    public string Id { get; set; }
    public Memory<byte> Data { get; set; }
}