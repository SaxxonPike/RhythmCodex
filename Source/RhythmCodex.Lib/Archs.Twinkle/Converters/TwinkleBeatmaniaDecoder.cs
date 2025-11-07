using System;
using System.IO;
using System.Linq;
using RhythmCodex.Archs.Twinkle.Heuristics;
using RhythmCodex.Archs.Twinkle.Model;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Pc.Converters;
using RhythmCodex.Games.Beatmania.Pc.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Riff.Converters;
using RhythmCodex.Sounds.Riff.Processing;
using RhythmCodex.Sounds.Riff.Streamers;

namespace RhythmCodex.Archs.Twinkle.Converters;

[Service]
public class TwinkleBeatmaniaDecoder(
    ITwinkleBeatmaniaSoundDefinitionDecoder twinkleBeatmaniaSoundDefinitionDecoder,
    ITwinkleBeatmaniaSoundDecoder twinkleBeatmaniaSoundDecoder,
    ITwinkleBeatmaniaChartDecoder twinkleBeatmaniaChartDecoder,
    ITwinkleBeatmaniaChartEventConverter twinkleBeatmaniaChartEventConverter,
    IBeatmaniaPc1ChartDecoder beatmaniaPc1ChartDecoder,
    ITwinkleBeatmaniaChartMetadataDecoder twinkleBeatmaniaChartMetadataDecoder,
    ITwinkleBeatmaniaChartHeuristic twinkleBeatmaniaChartHeuristic,
    IRiffStreamWriter riffStreamWriter,
    IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
    ISoundConsolidator soundConsolidator)
    : ITwinkleBeatmaniaDecoder
{
    private static readonly int[] ChartOffsets = Enumerable
        .Range(0, 0x3F)
        .Select(i => i * 0x4000 + 0x2000)
        .ToArray();

    public TwinkleArchive? Decode(TwinkleBeatmaniaChunk chunk, TwinkleDecodeOptions options)
    {
        var data = chunk.Data;

        if (data.Length < 0x1A00000)
            return new TwinkleArchive();

        var definitions = Enumerable.Range(0, 255)
            .ToDictionary(i => i,
                i => twinkleBeatmaniaSoundDefinitionDecoder.Decode(data.Span[(i * 0x12)..]));

        var sounds = definitions
            .Where(def => def.Value is { SizeInBlocks: > 0 })
            .Select(def =>
            {
                var sample = twinkleBeatmaniaSoundDecoder.Decode(def.Value!, data.Span[0x100000..]);
                if (sample != null)
                    sample[NumericData.Id] = def.Key + 1;
                return sample;
            })
            .Where(s => s != null)
            .Select(s => s!)
            .ToList();

        var charts = ChartOffsets
            .Select((offset, index) =>
            {
                if (twinkleBeatmaniaChartHeuristic.Match(data.Slice(offset, 0x4000)) == null)
                    return null;

                var events = twinkleBeatmaniaChartDecoder
                    .Decode(chunk.Data.Span.Slice(offset, 0x4000))
                    .Select(twinkleBeatmaniaChartEventConverter.ConvertToBeatmaniaPc1)
                    .ToList();

                if (!events.Any())
                    return null;

                var chart = beatmaniaPc1ChartDecoder.Decode(events, TwinkleConstants.BeatmaniaRate);
                chart[NumericData.ByteOffset] = offset;
                chart[NumericData.Id] = index;
                twinkleBeatmaniaChartMetadataDecoder.AddMetadata(chart, index);

                return chart;
            })
            .Where(c => c != null)
            .Select(c => c!)
            .ToList();

        if (!options.DoNotConsolidateSamples)
            soundConsolidator.Consolidate(sounds,
                charts.SelectMany(dc => dc?.Events ?? Enumerable.Empty<Event>()));

        return new TwinkleArchive
        {
            Charts = charts,
            Samples = sounds
        };
    }

    public BeatmaniaPcSongSet MigrateToBemaniPc(TwinkleBeatmaniaChunk chunk)
    {
        var data = chunk.Data;

        if (data.Length < 0x1A00000)
            return new BeatmaniaPcSongSet();

        data = data[..0x1A00000];

        var definitions = Enumerable.Range(0, 255)
            .ToDictionary(i => i,
                i => twinkleBeatmaniaSoundDefinitionDecoder.Decode(data.Span[(i * 0x12)..]));

        var sounds = definitions
            .Select(def =>
            {
                var source = def.Value != null
                    ? twinkleBeatmaniaSoundDecoder.Decode(def.Value, data.Span[0x100000..])
                    : new Sound { Samples = [new Sample { Data = Memory<float>.Empty }], [NumericData.Rate] = 44100 };

                using var mem = new MemoryStream();
                riffStreamWriter.Write(mem, riffPcm16SoundEncoder.Encode(source));
                mem.Flush();
                return new BeatmaniaPcAudioEntry
                {
                    Channel = def.Value?.Channel ?? 255,
                    Data = mem.ToArray(),
                    ExtraInfo = Memory<byte>.Empty,
                    Panning = def.Value?.Panning ?? 0x40,
                    Volume = def.Value?.Volume ?? 0x01
                };
            })
            .ToList();

        var charts = ChartOffsets
            .Select((offset, index) =>
            {
                if (twinkleBeatmaniaChartHeuristic.Match(data.Slice(offset, 0x4000)) == null)
                    return null;

                var events = twinkleBeatmaniaChartDecoder
                    .Decode(data.Span.Slice(offset, 0x4000))
                    .Select(twinkleBeatmaniaChartEventConverter.ConvertToBeatmaniaPc1)
                    .ToList();

                if (!events.Any())
                    return null;

                var noteCountEvents =
                    twinkleBeatmaniaChartEventConverter.ConvertNoteCountsToBeatmaniaPc1(
                        twinkleBeatmaniaChartDecoder.GetNoteCounts(chunk.Data.Span.Slice(offset, 0x4000)));

                return new BeatmaniaPc1Chart
                {
                    Index = index,
                    Data = noteCountEvents.Concat(events).ToList()
                };
            })
            .Where(c => c != null)
            .Select(c => c!)
            .ToList();

        return new BeatmaniaPcSongSet
        {
            Charts = charts,
            Sounds = sounds
        };
    }
}