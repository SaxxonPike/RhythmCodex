using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XwbHeaderStreamReader : IXwbHeaderStreamReader
    {
        private readonly IXwbRegionStreamReader _xwbRegionStreamReader;

        public XwbHeaderStreamReader(IXwbRegionStreamReader xwbRegionStreamReader)
        {
            _xwbRegionStreamReader = xwbRegionStreamReader;
        }
        
        public WaveBankHeader Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var result = new WaveBankHeader
            {
                Signature = reader.ReadInt32(),
                Version = reader.ReadInt32(),
                HeaderVersion = reader.ReadInt32(),
                Segments = new WaveBankRegion[(int) WaveBankSegIdx.Count]
            };

            for (var i = 0; i < result.Segments.Length; i++)
                result.Segments[i] = _xwbRegionStreamReader.Read(source);

            return result;
        }
    }
}