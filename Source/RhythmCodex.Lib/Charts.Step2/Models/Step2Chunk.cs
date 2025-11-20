using System;
using System.Collections.Generic;

namespace RhythmCodex.Charts.Step2.Models;

public class Step2Chunk
{
    public List<Step2Metadata> Metadatas { get; set; } = [];
    public Memory<byte> Data { get; set; }
}