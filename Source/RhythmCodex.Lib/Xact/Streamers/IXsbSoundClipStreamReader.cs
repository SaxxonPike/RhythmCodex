using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXsbSoundClipStreamReader
    {
        XsbSoundClip[] Read(Stream stream, int count);
    }
}