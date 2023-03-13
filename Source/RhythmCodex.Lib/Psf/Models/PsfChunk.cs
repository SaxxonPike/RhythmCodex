using RhythmCodex.Infrastructure;

namespace RhythmCodex.Psf.Models;

[Model]
public class PsfChunk
{
    public int Version { get; set; }
    public int Crc { get; set; }
    public byte[] Reserved { get; set; }
    public byte[] Data { get; set; }
}