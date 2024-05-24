using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers;

[Service]
public class XwbMiniWaveFormatStreamReader : IXwbMiniWaveFormatStreamReader
{
    public XwbMiniWaveFormat Read(Stream source)
    {
        var reader = new BinaryReader(source);
        var result = new XwbMiniWaveFormat {Value = reader.ReadInt32()};
        return result;
    }
}