using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers;

[Service]
public class XwbHeaderStreamReader(IXwbRegionStreamReader xwbRegionStreamReader) : IXwbHeaderStreamReader
{
    public XwbHeader Read(Stream source)
    {
        var reader = new BinaryReader(source);
        var result = new XwbHeader
        {
            Signature = reader.ReadInt32(),
            Version = reader.ReadInt32(),
            HeaderVersion = reader.ReadInt32(),
            Segments = new XwbRegion[(int) XwbSegIdx.Count]
        };

        for (var i = 0; i < result.Segments.Length; i++)
            result.Segments[i] = xwbRegionStreamReader.Read(source);

        return result;
    }
}