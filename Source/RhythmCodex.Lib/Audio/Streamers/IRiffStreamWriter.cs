using System.IO;
using RhythmCodex.Audio.Models;

namespace RhythmCodex.Audio.Streamers
{
    public interface IRiffStreamWriter
    {
        int Write(Stream stream, IRiffContainer container);
    }
}