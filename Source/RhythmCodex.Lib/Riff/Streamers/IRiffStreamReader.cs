using System.IO;
using RhythmCodex.Riff.Models;

namespace RhythmCodex.Riff.Streamers;

public interface IRiffStreamReader
{
    IRiffContainer Read(Stream stream);
}