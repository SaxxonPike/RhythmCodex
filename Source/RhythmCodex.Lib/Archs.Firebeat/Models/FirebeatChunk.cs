using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Firebeat.Models;

[Model]
public class FirebeatChunk
{
    public FirebeatChunkFormat Format { get; set; }
    public Memory<byte> Data { get; set; }
    public int Id { get; set; }

    public override string ToString() => Json.Serialize(this);
}