using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XwbRegionStreamReader : IXwbRegionStreamReader
    {
        public WaveBankRegion Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var result = new WaveBankRegion {Offset = reader.ReadInt32(), Length = reader.ReadInt32()};
            return result;
        }
    }
}