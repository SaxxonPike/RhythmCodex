using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public record DjmainChunk
{
    public DjmainChunkFormat Format { get; init; }
    public Memory<byte> Data { get; init; }
    public int Id { get; init; }
        
    public override string ToString() => Json.Serialize(this);
}