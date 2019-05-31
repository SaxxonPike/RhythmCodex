using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Djmain.Heuristics;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Djmain.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Processing;
using RhythmCodex.Sounds.Models;
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
        private readonly IDjmainUsedSampleFilter _usedSampleFilter;
        private readonly ISoundConsolidator _soundConsolidator;
        private readonly IDjmainChartMetadataDecoder _djmainChartMetadataDecoder;

        public DjmainDecoder(
            IDjmainChartDecoder chartDecoder,
            IDjmainChartEventStreamReader chartEventStreamReader,
            IDjmainOffsetProvider offsetProvider,
            IDjmainSoundDecoder soundDecoder,
            IDjmainSampleInfoStreamReader sampleInfoStreamReader,
            IDjmainSampleDecoder sampleDecoder,
            IDjmainUsedSampleFilter usedSampleFilter,
            ISoundConsolidator soundConsolidator,
            IDjmainChartMetadataDecoder djmainChartMetadataDecoder)
        {
            _chartDecoder = chartDecoder;
            _chartEventStreamReader = chartEventStreamReader;
            _offsetProvider = offsetProvider;
            _soundDecoder = soundDecoder;
            _sampleInfoStreamReader = sampleInfoStreamReader;
            _sampleDecoder = sampleDecoder;
            _usedSampleFilter = usedSampleFilter;
            _soundConsolidator = soundConsolidator;
            _djmainChartMetadataDecoder = djmainChartMetadataDecoder;
        }

        public IDjmainArchive Decode(IDjmainChunk chunk)
        {
            if (chunk.Data.Length != 0x1000000)
                throw new RhythmCodexException("Chunk length must be exactly 16mb (0x1000000 bytes)");

            if (chunk.Format == DjmainChunkFormat.Unknown ||
                !Enum.IsDefined(typeof(DjmainChunkFormat), chunk.Format))
                throw new RhythmCodexException($"{nameof(chunk.Format)} is not recognized");

            using (var stream = new ReadOnlyMemoryStream(chunk.Data))
            {
                var swappedStream = new ByteSwappedReadStream(stream);

                var chartSoundMap = _offsetProvider.GetSampleChartMap(chunk.Format)
                    .Select((offset, index) => new KeyValuePair<int, int>(index, offset))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
                var rawCharts = ExtractCharts(stream, chunk.Format).Where(c => c.Value != null).ToList();
                var decodedCharts = DecodeCharts(rawCharts, chartSoundMap, chunk.Format);
                var sounds = DecodeSounds(swappedStream, chunk.Format, chartSoundMap, rawCharts, decodedCharts)
                    .ToDictionary(kv => kv.Key, kv => kv.Value.Select(s => s));

                return new DjmainArchive
                {
                    Charts = decodedCharts.Select(c => c.Value).ToList(),
                    Samples = sounds.SelectMany(s => s.Value).Select(s => s.Value).ToList()
                };
            }
        }

        private DjmainChartType GetChartType(DjmainChunkFormat format)
        {
            switch (format)
            {
                case DjmainChunkFormat.Popn1:
                case DjmainChunkFormat.Popn2:
                case DjmainChunkFormat.Popn3:
                    return DjmainChartType.Popn;
                case DjmainChunkFormat.BeatmaniaClub:
                case DjmainChunkFormat.BeatmaniaComplete:
                case DjmainChunkFormat.BeatmaniaComplete2:
                case DjmainChunkFormat.BeatmaniaCore:
                case DjmainChunkFormat.BeatmaniaDct:
                case DjmainChunkFormat.BeatmaniaFifth:
                case DjmainChunkFormat.BeatmaniaFinal:
                case DjmainChunkFormat.BeatmaniaFirst:
                case DjmainChunkFormat.BeatmaniaFourth:
                case DjmainChunkFormat.BeatmaniaSecond:
                case DjmainChunkFormat.BeatmaniaSeventh:
                case DjmainChunkFormat.BeatmaniaSixth:
                case DjmainChunkFormat.BeatmaniaThird:
                    return DjmainChartType.Beatmania;
                default:
                    throw new RhythmCodexException($"Not sure what chart type this format is: {format}");
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

        private IDictionary<int, IChart> DecodeCharts(
            IEnumerable<KeyValuePair<int, IEnumerable<IDjmainChartEvent>>> events,
            IReadOnlyDictionary<int, int> chartSoundMap, DjmainChunkFormat chunkFormat)
        {
            return events.ToDictionary(x => x.Key, x =>
            {
                if (x.Value == null)
                    return null;

                var chart = _chartDecoder.Decode(x.Value, GetChartType(chunkFormat));
                chart[NumericData.Id] = x.Key;
                chart[NumericData.SampleMap] = chartSoundMap[x.Key];
                _djmainChartMetadataDecoder.AddMetadata(chart, chunkFormat, x.Key);
                return chart;
            });
        }

        private IDictionary<int, IDictionary<int, ISound>> DecodeSounds(
            Stream stream,
            DjmainChunkFormat format,
            IReadOnlyDictionary<int, int> chartSoundMap,
            IEnumerable<KeyValuePair<int, IEnumerable<IDjmainChartEvent>>> charts,
            IEnumerable<KeyValuePair<int, IChart>> decodedCharts)
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
                    var decodedSamples = _soundDecoder.Decode(samples);
                    _soundConsolidator.Consolidate(decodedSamples.Values,
                        decodedCharts.SelectMany(dc => dc.Value?.Events ?? Enumerable.Empty<IEvent>()));

                    foreach (var sample in decodedSamples.Where(s => s.Value.Samples.Any()))
                    {
                        var s = sample.Value;
                        s[NumericData.SampleMap] = kv.Key;
                    }

                    foreach (var discardedSample in decodedSamples.Where(s => !s.Value.Samples.Any()).ToList())
                        decodedSamples.Remove(discardedSample);

                    return decodedSamples;
                });
        }

        private IDictionary<int, IDjmainSample> DecodeSamples(Stream stream, DjmainChunkFormat format,
            int sampleMapOffset, IEnumerable<IDjmainChartEvent> events)
        {
            stream.Position = sampleMapOffset;
            var infos = _sampleInfoStreamReader.Read(stream);
            var filteredInfos = _usedSampleFilter.Filter(infos, events);
            return _sampleDecoder.Decode(stream, filteredInfos, _offsetProvider.GetSoundOffset(format));
        }
    }
}