using System;
using System.IO;
using System.Linq;
using RhythmCodex.Beatmania.Converters;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Twinkle.Heuristics;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters;

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
    IRiffPcm16SoundEncoder riffPcm16SoundEncoder)
    : ITwinkleBeatmaniaDecoder
{
    private static readonly int[] ChartOffsets = Enumerable
        .Range(0, 0x3F)
        .Select(i => i * 0x4000 + 0x2000)
        .ToArray();

    public TwinkleArchive? Decode(TwinkleBeatmaniaChunk chunk)
    {
        if (chunk?.Data == null || chunk.Data.Length < 0x1A00000)
            return null;

        var definitions = Enumerable.Range(0, 255)
            .ToDictionary(i => i,
                i => twinkleBeatmaniaSoundDefinitionDecoder.Decode(chunk.Data.AsSpan(i * 0x12)));

        var sounds = definitions
            .Where(def => def.Value is { SizeInBlocks: > 0 })
            .Select(def =>
            {
                var sample = twinkleBeatmaniaSoundDecoder.Decode(def.Value, chunk.Data.AsSpan(0x100000));
                sample[NumericData.Id] = def.Key + 1;
                return sample;
            })
            .ToList();

        var charts = ChartOffsets
            .Select((offset, index) =>
            {
                if (twinkleBeatmaniaChartHeuristic.Match(chunk.Data.AsMemory(offset, 0x4000)) == null)
                    return null;

                var events = twinkleBeatmaniaChartDecoder
                    .Decode(chunk.Data.AsSpan(offset), 0x4000)
                    .Select(twinkleBeatmaniaChartEventConverter.ConvertToBeatmaniaPc1)
                    .ToList();

                if (!events.Any())
                    return null;

                var chart = beatmaniaPc1ChartDecoder.Decode(events, TwinkleConstants.BeatmaniaRate);
                if (chart != null)
                {
                    chart[NumericData.ByteOffset] = offset;
                    chart[NumericData.Id] = index;
                    twinkleBeatmaniaChartMetadataDecoder.AddMetadata(chart, index);
                }

                return chart;
            })
            .Where(c => c != null)
            .ToList();

        return new TwinkleArchive
        {
            Charts = charts,
            Samples = sounds
        };
    }

    public BeatmaniaPcSongSet MigrateToBemaniPc(TwinkleBeatmaniaChunk chunk)
    {
        if (chunk?.Data == null || chunk.Data.Length < 0x1A00000)
            return null;

        var definitions = Enumerable.Range(0, 255)
            .ToDictionary(i => i,
                i => twinkleBeatmaniaSoundDefinitionDecoder.Decode(chunk.Data.AsSpan(i * 0x12)));

        var sounds = definitions
            .Select(def =>
            {
                var source = def.Value != null
                    ? twinkleBeatmaniaSoundDecoder.Decode(def.Value, chunk.Data.AsSpan(0x100000))
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
                if (twinkleBeatmaniaChartHeuristic.Match(chunk.Data.AsMemory(offset, 0x4000)) == null)
                    return null;

                var events = twinkleBeatmaniaChartDecoder
                    .Decode(chunk.Data.AsSpan(offset), 0x4000)
                    .Select(twinkleBeatmaniaChartEventConverter.ConvertToBeatmaniaPc1)
                    .ToList();

                if (!events.Any())
                    return null;

                var noteCountEvents =
                    twinkleBeatmaniaChartEventConverter.ConvertNoteCountsToBeatmaniaPc1(
                        twinkleBeatmaniaChartDecoder.GetNoteCounts(chunk.Data.AsSpan(offset), 0x4000));

                return new BeatmaniaPc1Chart
                {
                    Index = index,
                    Data = noteCountEvents.Concat(events).ToList()
                };
            })
            .Where(c => c != null)
            .ToList();

        return new BeatmaniaPcSongSet
        {
            Charts = charts,
            Sounds = sounds
        };
    }
}