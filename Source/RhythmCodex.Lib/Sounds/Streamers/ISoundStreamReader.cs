using System.IO;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Streamers;

public interface ISoundStreamReader
{
    Sound? Read(Stream stream);
}