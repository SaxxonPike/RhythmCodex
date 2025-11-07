using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

[Service]
public class XwbSampleRegionStreamReader : IXwbSampleRegionStreamReader
{
    public XwbSampleRegion Read(Stream source)
    {
        var reader = new BinaryReader(source);
        var result = new XwbSampleRegion
        {
            StartSample = reader.ReadInt32(),
            TotalSamples = reader.ReadInt32()
        };
        return result;
    }
}