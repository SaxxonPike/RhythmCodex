using System;

namespace RhythmCodex.Riff.Models;

public interface IRiffChunk
{
    string Id { get; }
    Memory<byte> Data { get; }
}