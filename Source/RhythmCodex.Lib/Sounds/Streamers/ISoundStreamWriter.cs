using System.IO;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Streamers;

public interface ISoundStreamWriter
{
    void Write(Stream stream, Sound sound);
}