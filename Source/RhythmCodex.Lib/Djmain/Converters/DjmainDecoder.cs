using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Djmain.Heuristics;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Djmain.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.Streamers;

namespace RhythmCodex.Djmain.Converters
{
    [Service]
    public class DjmainDecoder : IDjmainDecoder
    {
        private readonly IDjmainChartDecoder _djmainChartDecoder;
        private readonly IDjmainChartEventStreamReader _djmainChartEventStreamReader;
        private readonly IDjmainOffsetProvider _djmainOffsetProvider;

        public DjmainDecoder(
            IDjmainChartDecoder djmainChartDecoder,
            IDjmainChartEventStreamReader djmainChartEventStreamReader,
            IDjmainOffsetProvider djmainOffsetProvider)
        {
            _djmainChartDecoder = djmainChartDecoder;
            _djmainChartEventStreamReader = djmainChartEventStreamReader;
            _djmainOffsetProvider = djmainOffsetProvider;
        }

        public IDjmainArchive Decode(IDjmainChunk djmainChunk)
        {
            if (djmainChunk.Data.Length != 0x1000000)
                throw new RhythmCodexException("Chunk length must be exactly 16mb (0x1000000 bytes)");

            if (djmainChunk.Format == DjmainChunkFormat.Unknown ||
                !Enum.IsDefined(typeof(DjmainChunkFormat), djmainChunk.Format))
                throw new RhythmCodexException($"{nameof(djmainChunk.Format)} is not recognized");

            using (var stream = new MemoryStream(djmainChunk.Data))
            {
                var swappedStream = new ByteSwappedReadStream(stream);

                var charts = DecodeCharts(stream, djmainChunk.Format);
                
                return new DjmainArchive
                {
                    Charts = charts.Values.ToList()
                };
            }
        }

        private IDictionary<int, IChart> DecodeCharts(Stream stream, DjmainChunkFormat format)
        {
            return _djmainOffsetProvider.GetChartOffsets(format)
                .Select((offset, index) => new KeyValuePair<int, int>(index, offset))
                .ToDictionary(kv => kv.Key, kv =>
                {
                    stream.Position = kv.Value;
                    var events = _djmainChartEventStreamReader.Read(stream);
                    if (events.Count == 0 || events[0].Offset != 0 || (events[0].Param0 == 0 && events[0].Param1 == 0))
                        return null;
                    var chart = _djmainChartDecoder.Decode(events);
                    chart[NumericData.Id] = kv.Key;
                    chart[NumericData.ByteOffset] = kv.Value;
                    return chart;
                });
        }
    }
}