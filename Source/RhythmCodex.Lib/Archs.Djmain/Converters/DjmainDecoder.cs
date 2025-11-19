using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Archs.Djmain.Heuristics;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Archs.Djmain.Streamers;
using RhythmCodex.Charts.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Riff.Processing;
using RhythmCodex.Streamers;

namespace RhythmCodex.Archs.Djmain.Converters;

[Service]
public class DjmainDecoder(
    IDjmainChartDecoder chartDecoder,
    IDjmainChartEventStreamReader chartEventStreamReader,
    IDjmainOffsetProvider offsetProvider,
    IDjmainSoundDecoder soundDecoder,
    IDjmainSampleInfoStreamReader sampleInfoStreamReader,
    IDjmainSampleDecoder sampleDecoder,
    IDjmainUsedSampleFilter usedSampleFilter,
    ISoundConsolidator soundConsolidator,
    IDjmainChartMetadataDecoder djmainChartMetadataDecoder)
    : IDjmainDecoder
{
    public DjmainArchive Decode(DjmainChunk chunk, DjmainDecodeOptions options)
    {
        if (chunk.Data.Length != 0x1000000)
            throw new RhythmCodexException("Chunk length must be exactly 16mb (0x1000000 bytes)");

        if (chunk.Format == DjmainChunkFormat.Unknown ||
            !Enum.IsDefined(chunk.Format))
            throw new RhythmCodexException($"{nameof(chunk.Format)} is not recognized");

        using var stream = new ReadOnlyMemoryStream(chunk.Data);
        var swappedStream = new ByteSwappedReadStream(stream);

        var chartSoundMap = offsetProvider.GetSampleChartMap(chunk.Format)
            .Select((offset, index) => new KeyValuePair<int, int>(index, offset))
            .ToDictionary(kv => kv.Key, kv => kv.Value);
        var rawCharts = ExtractCharts(stream, chunk.Format);
        var decodedCharts = DecodeCharts(rawCharts, chartSoundMap, chunk.Format);
        var sounds = options.DisableAudio
            ? new Dictionary<int, IEnumerable<KeyValuePair<int, Sound>>>()
            : DecodeSounds(swappedStream, chunk.Format, chartSoundMap, rawCharts, decodedCharts, options)
                .ToDictionary(kv => kv.Key, kv => kv.Value.Select(s => s));

        return new DjmainArchive
        {
            RawCharts = rawCharts.ToDictionary(kv => kv.Key, kv => kv.Value),
            Charts = decodedCharts.Select(c => c.Value).ToList(),
            Samples = sounds.SelectMany(s => s.Value).Select(s => s.Value).ToList()
        };
    }

    private static DjmainChartType GetChartType(DjmainChunkFormat format)
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

    private Dictionary<int, List<DjmainChartEvent>> ExtractCharts(
        Stream stream,
        DjmainChunkFormat format) =>
        offsetProvider
            .GetChartOffsets(format)
            .Select((offset, index) => new KeyValuePair<int, int>(index, offset))
            .Select(kv =>
            {
                stream.Position = kv.Value;
                var events = chartEventStreamReader.Read(stream);
                if (events.Count == 0 || events[0].Offset != 0 || (events[0].Param0 == 0 && events[0].Param1 == 0))
                    return (kv.Key, null);
                return (kv.Key, Events: events);
            })
            .Where(kv => kv.Events != null)
            .ToDictionary(kv => kv.Key, kv => kv.Events!);

    private Dictionary<int, Chart> DecodeCharts(
        Dictionary<int, List<DjmainChartEvent>> events,
        Dictionary<int, int> chartSoundMap,
        DjmainChunkFormat chunkFormat) =>
        events.ToDictionary(x => x.Key, x =>
        {
            var chart = chartDecoder.Decode(x.Value, GetChartType(chunkFormat));
            chart[NumericData.Id] = x.Key;
            chart[NumericData.SampleMap] = chartSoundMap[x.Key];
            chart[NumericData.PriorityChannels] = 8;
            djmainChartMetadataDecoder.AddMetadata(chart, chunkFormat, x.Key);
            return chart;
        });

    private Dictionary<int, Dictionary<int, Sound>> DecodeSounds(Stream stream,
        DjmainChunkFormat format,
        Dictionary<int, int> chartSoundMap,
        Dictionary<int, List<DjmainChartEvent>> charts,
        Dictionary<int, Chart> decodedCharts,
        DjmainDecodeOptions options) =>
        offsetProvider.GetSampleMapOffsets(format)
            .Select((offset, index) => new KeyValuePair<int, int>(index, offset))
            .ToDictionary(kv => kv.Key, kv =>
            {
                var chartData = charts
                    .Where(c => chartSoundMap[c.Key] == kv.Key)
                    .SelectMany(c => c.Value)
                    .ToList();
                var samples = DecodeSamples(stream, format, kv.Value, chartData);
                var decodedSamples = soundDecoder.Decode(samples);
                
                foreach (var sample in decodedSamples.Where(s => s.Value.Samples.Count != 0))
                {
                    var s = sample.Value;
                    s[NumericData.SampleMap] = kv.Key;
                }

                if (!options.DoNotConsolidateSamples)
                    soundConsolidator.Consolidate(decodedSamples.Values, decodedCharts.Values);

                foreach (var discardedSample in decodedSamples
                             .Where(s => s.Value.Samples.Count == 0)
                             .ToList())
                    decodedSamples.Remove(discardedSample.Key);

                return decodedSamples;
            });

    private Dictionary<int, DjmainSample> DecodeSamples(Stream stream, DjmainChunkFormat format,
        int sampleMapOffset, IEnumerable<DjmainChartEvent> events)
    {
        stream.Position = sampleMapOffset;
        var infos = sampleInfoStreamReader.Read(stream);
        var filteredInfos = usedSampleFilter.Filter(infos, events);
        return sampleDecoder.Decode(stream, filteredInfos, offsetProvider.GetSoundOffset(format));
    }
}