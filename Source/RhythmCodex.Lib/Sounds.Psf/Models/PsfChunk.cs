using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Psf.Models;

[Model]
public class PsfChunk
{
    public int Version { get; set; }
    public int Crc { get; set; }
    public Memory<byte> Reserved { get; set; }
    public Memory<byte> Data { get; set; }
}