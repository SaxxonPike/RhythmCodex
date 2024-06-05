using System;

namespace RhythmCodex.Ssq.Model;

public class SsqInfoChunk
{
    public string[] Text { get; set; } = [];
    public Memory<byte> Difficulties { get; set; }
}