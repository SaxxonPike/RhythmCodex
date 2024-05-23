using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers;

[Service]
public class XwbEntryStreamReader(
    IXwbMiniWaveFormatStreamReader xwbMiniWaveFormatStreamReader,
    IXwbRegionStreamReader xwbRegionStreamReader,
    IXwbSampleRegionStreamReader xwbSampleRegionStreamReader)
    : IXwbEntryStreamReader
{
    public XwbEntry Read(Stream source)
    {
        var reader = new BinaryReader(source);
        var result = new XwbEntry
        {
            Value = reader.ReadInt32(),
            Format = xwbMiniWaveFormatStreamReader.Read(source),
            PlayRegion = xwbRegionStreamReader.Read(source),
            LoopRegion = xwbSampleRegionStreamReader.Read(source)
        };

        return result;
    }
}