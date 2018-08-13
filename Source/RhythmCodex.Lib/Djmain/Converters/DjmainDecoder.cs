using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Audio;
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
        private readonly IDjmainChartDecoder _chartDecoder;
        private readonly IDjmainChartEventStreamReader _chartEventStreamReader;
        private readonly IDjmainOffsetProvider _offsetProvider;
        private readonly IDjmainSoundDecoder _soundDecoder;
        private readonly IDjmainSampleInfoStreamReader _sampleInfoStreamReader;
        private readonly IDjmainSampleDecoder _sampleDecoder;
        private readonly IDjmainSampleMapConsolidator _sampleMapConsolidator;
        private readonly IDjmainUsedSampleFilter _usedSampleFilter;

        public DjmainDecoder(
            IDjmainChartDecoder chartDecoder,
            IDjmainChartEventStreamReader chartEventStreamReader,
            IDjmainOffsetProvider offsetProvider,
            IDjmainSoundDecoder soundDecoder,
            IDjmainSampleInfoStreamReader sampleInfoStreamReader,
            IDjmainSampleDecoder sampleDecoder,
            IDjmainSampleMapConsolidator sampleMapConsolidator,
            IDjmainUsedSampleFilter usedSampleFilter)
        {
            _chartDecoder = chartDecoder;
            _chartEventStreamReader = chartEventStreamReader;
            _offsetProvider = offsetProvider;
            _soundDecoder = soundDecoder;
            _sampleInfoStreamReader = sampleInfoStreamReader;
            _sampleDecoder = sampleDecoder;
            _sampleMapConsolidator = sampleMapConsolidator;
            _usedSampleFilter = usedSampleFilter;
        }

        public IDjmainArchive Decode(IDjmainChunk chunk)
        {
            if (chunk.Data.Length != 0x1000000)
                throw new RhythmCodexException("Chunk length must be exactly 16mb (0x1000000 bytes)");

            if (chunk.Format == DjmainChunkFormat.Unknown ||
                !Enum.IsDefined(typeof(DjmainChunkFormat), chunk.Format))
                throw new RhythmCodexException($"{nameof(chunk.Format)} is not recognized");

            using (var stream = new MemoryStream(chunk.Data))
            {
                var swappedStream = new ByteSwappedReadStream(stream);

                var chartSoundMap = _offsetProvider.GetSampleChartMap(chunk.Format)
                    .Select((offset, index) => new KeyValuePair<int, int>(index, offset))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
                var rawCharts = ExtractCharts(stream, chunk.Format);
                var decodedCharts = DecodeCharts(rawCharts);
                var sounds = DecodeSounds(swappedStream, chunk.Format, chartSoundMap, rawCharts)
                    .ToDictionary(kv => kv.Key, kv => kv.Value.Select(s => s));
                //var consolidated = _sampleMapConsolidator.Consolidate(sounds, decodedCharts, chartSoundMap);
                
                return new DjmainArchive
                {
                    Charts = decodedCharts.Select(c => c.Value).ToList(),
                    Samples = sounds.First().Value.Select(s => s.Value).ToList()
                };
            }
        }

        private IDictionary<int, IEnumerable<IDjmainChartEvent>> ExtractCharts(Stream stream, DjmainChunkFormat format)
        {
            return _offsetProvider.GetChartOffsets(format)
                .Select((offset, index) => new KeyValuePair<int, int>(index, offset))
                .ToDictionary(kv => kv.Key, kv =>
                {
                    stream.Position = kv.Value;
                    var events = _chartEventStreamReader.Read(stream);
                    if (events.Count == 0 || events[0].Offset != 0 || (events[0].Param0 == 0 && events[0].Param1 == 0))
                        return null;
                    return events.Cast<IDjmainChartEvent>();
                });
            
        }

        private IDictionary<int, IChart> DecodeCharts(IDictionary<int, IEnumerable<IDjmainChartEvent>> events)
        {
            return events.ToDictionary(x => x.Key, x =>
            {
                var chart = _chartDecoder.Decode(x.Value);
                chart[NumericData.Id] = x.Key;
                return chart;
            });
        }
        
        private IDictionary<int, IDictionary<int, ISound>> DecodeSounds(Stream stream, DjmainChunkFormat format,
            IReadOnlyDictionary<int, int> chartSoundMap, IDictionary<int, IEnumerable<IDjmainChartEvent>> charts)
        {
            return _offsetProvider.GetSampleMapOffsets(format)
                .Select((offset, index) => new KeyValuePair<int, int>(index, offset))
                .ToDictionary(kv => kv.Key, kv =>
                {
                    var chartData = charts
                        .Where(c => chartSoundMap[c.Key] == kv.Key)
                        .SelectMany(c => c.Value)
                        .ToList();
                    var samples = DecodeSamples(stream, format, kv.Value, chartData);
                    return _soundDecoder.Decode(samples);
                });
        }

        private IDictionary<int, IDjmainSample> DecodeSamples(Stream stream, DjmainChunkFormat format, int sampleMapOffset, IEnumerable<IDjmainChartEvent> events)
        {
            stream.Position = sampleMapOffset;
            var infos = _sampleInfoStreamReader.Read(stream);
            var filteredInfos = _usedSampleFilter.Filter(infos, events);
            return _sampleDecoder.Decode(stream, filteredInfos, _offsetProvider.GetSoundOffset(format));
        }
    }
}