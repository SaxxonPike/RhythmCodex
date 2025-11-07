using System.IO;
using RhythmCodex.Sounds.Psf.Models;

namespace RhythmCodex.Sounds.Psf.Streamers;

public interface IPsfStreamReader
{
    PsfChunk Read(Stream source);
}