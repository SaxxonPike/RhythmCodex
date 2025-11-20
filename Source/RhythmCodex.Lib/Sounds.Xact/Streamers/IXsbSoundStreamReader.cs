using System.IO;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

public interface IXsbSoundStreamReader
{
    XsbSound Read(Stream stream);
}