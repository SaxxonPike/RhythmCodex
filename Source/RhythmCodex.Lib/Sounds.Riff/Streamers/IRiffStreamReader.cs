using System.IO;
using RhythmCodex.Sounds.Riff.Models;

namespace RhythmCodex.Sounds.Riff.Streamers;

public interface IRiffStreamReader
{
    RiffContainer Read(Stream stream);
}