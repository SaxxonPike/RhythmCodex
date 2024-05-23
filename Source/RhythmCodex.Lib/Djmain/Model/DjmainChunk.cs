using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public class DjmainChunk
{
    public DjmainChunkFormat Format { get; set; }
    public Memory<byte> Data { get; set; }
    public int Id { get; set; }
        
    public override string ToString() => Json.Serialize(this);
}