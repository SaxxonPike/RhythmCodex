using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
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
}