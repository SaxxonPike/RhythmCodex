using System.IO;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Streamers;

public interface IVagStreamReader
{
    VagChunk Read(Stream stream, int channels, int interleave);
}