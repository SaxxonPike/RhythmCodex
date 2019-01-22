using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXwbRegionStreamReader
    {
        WaveBankRegion Read(Stream source);
    }
}