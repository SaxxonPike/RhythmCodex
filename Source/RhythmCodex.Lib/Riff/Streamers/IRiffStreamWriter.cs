using System.IO;
using RhythmCodex.Riff.Models;

namespace RhythmCodex.Riff.Streamers;

public interface IRiffStreamWriter
{
    int Write(Stream stream, IRiffContainer container);
}