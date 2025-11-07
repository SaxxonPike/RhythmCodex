using System;

namespace RhythmCodex.Charts.Ssq.Model;

public class SsqInfoChunk
{
    public string[] Text { get; set; } = [];
    public Memory<byte> Difficulties { get; set; }
}