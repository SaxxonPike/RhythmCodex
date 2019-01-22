using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XwbSampleRegionStreamReader : IXwbSampleRegionStreamReader
    {
        public WaveBankSampleRegion Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var result = new WaveBankSampleRegion
            {
                StartSample = reader.ReadInt32(),
                TotalSamples = reader.ReadInt32()
            };
            return result;
        }
    }
}