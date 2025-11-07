using System.IO;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

public interface IXwbSampleRegionStreamReader
{
    XwbSampleRegion Read(Stream source);
}