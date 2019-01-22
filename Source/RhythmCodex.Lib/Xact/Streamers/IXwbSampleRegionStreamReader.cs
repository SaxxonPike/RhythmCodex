using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXwbSampleRegionStreamReader
    {
        WaveBankSampleRegion Read(Stream source);
    }
}