using System.IO;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

public interface IXsbSoundDspStreamReader
{
    XsbSoundDsp Read(Stream stream);
}