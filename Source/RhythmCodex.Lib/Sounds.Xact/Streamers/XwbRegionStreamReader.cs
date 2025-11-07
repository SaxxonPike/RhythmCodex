using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

[Service]
public class XwbRegionStreamReader : IXwbRegionStreamReader
{
    public XwbRegion Read(Stream source)
    {
        var reader = new BinaryReader(source);
        var result = new XwbRegion {Offset = reader.ReadInt32(), Length = reader.ReadInt32()};
        return result;
    }
}