using System.IO;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Sounds.Vag.Streamers;

public interface IVagStreamReader
{
    VagChunk? Read(Stream stream, int channels, int interleave);
}