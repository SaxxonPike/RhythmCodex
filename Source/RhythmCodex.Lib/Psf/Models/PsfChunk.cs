using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Psf.Models;

[Model]
public record PsfChunk
{
    public int Version { get; init; }
    public int Crc { get; init; }
    public ReadOnlyMemory<byte> Reserved { get; init; }
    public ReadOnlyMemory<byte> Data { get; init; }
}