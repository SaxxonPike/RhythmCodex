using System.IO;
using RhythmCodex.Sounds.Riff.Models;

namespace RhythmCodex.Sounds.Riff.Streamers;

public interface IRiffStreamWriter
{
    int Write(Stream stream, RiffContainer container);
}