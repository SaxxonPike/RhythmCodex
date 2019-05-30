using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
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
}