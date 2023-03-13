using System.IO;
using RhythmCodex.Psf.Models;

namespace RhythmCodex.Psf.Streamers;

public interface IPsfStreamReader
{
    PsfChunk Read(Stream source);
}