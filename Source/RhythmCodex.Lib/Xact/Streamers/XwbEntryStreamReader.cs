using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XwbEntryStreamReader : IXwbEntryStreamReader
    {
        private readonly IXwbMiniWaveFormatStreamReader _xwbMiniWaveFormatStreamReader;
        private readonly IXwbRegionStreamReader _xwbRegionStreamReader;
        private readonly IXwbSampleRegionStreamReader _xwbSampleRegionStreamReader;

        public XwbEntryStreamReader(
            IXwbMiniWaveFormatStreamReader xwbMiniWaveFormatStreamReader,
            IXwbRegionStreamReader xwbRegionStreamReader,
            IXwbSampleRegionStreamReader xwbSampleRegionStreamReader)
        {
            _xwbMiniWaveFormatStreamReader = xwbMiniWaveFormatStreamReader;
            _xwbRegionStreamReader = xwbRegionStreamReader;
            _xwbSampleRegionStreamReader = xwbSampleRegionStreamReader;
        }
        
        public XwbEntry Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var result = new XwbEntry
            {
                Value = reader.ReadInt32(),
                Format = _xwbMiniWaveFormatStreamReader.Read(source),
                PlayRegion = _xwbRegionStreamReader.Read(source),
                LoopRegion = _xwbSampleRegionStreamReader.Read(source)
            };

            return result;
        }
    }
}