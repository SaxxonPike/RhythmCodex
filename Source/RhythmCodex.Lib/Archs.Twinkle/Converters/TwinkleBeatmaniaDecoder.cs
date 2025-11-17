using System;
using System.Collections.Generic;
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
    ITwinkleBeatmaniaSoundDefinitionDecoder soundDefinitionDecoder,
    ITwinkleBeatmaniaSoundDecoder soundDecoder,
    ITwinkleBeatmaniaChartDecoder chartDecoder,
    ITwinkleBeatmaniaChartEventConverter chartEventConverter,
    IBeatmaniaPc1ChartDecoder pc1ChartDecoder,
    ITwinkleBeatmaniaChartMetadataDecoder chartMetadataDecoder,
    ITwinkleBeatmaniaChartHeuristic chartHeuristic,
    IRiffStreamWriter riffStreamWriter,
    IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
    ISoundConsolidator soundConsolidator)
    : ITwinkleBeatmaniaDecoder
{
    private static readonly int[] ChartOffsets = Enumerable
        .Range(0, 0x3F)
        .Select(i => i * 0x4000 + 0x2000)
        .ToArray();

    public TwinkleArchive Decode(TwinkleBeatmaniaChunk chunk, TwinkleDecodeOptions options)
    {
        var data = chunk.Data;

        if (data.Length < TwinkleConstants.ChunkSize)
            return new TwinkleArchive();

        var definitions = new Dictionary<int, TwinkleBeatmaniaSoundDefinition?>(256);

        for (var i = 0; i < 255; i++)
        {
            var def = soundDefinitionDecoder.Decode(data.Span[(i * 0x12)..]);
            definitions.Add(i, def);
        }

        var sounds = new List<Sound>(definitions.Count);

        foreach (var def in definitions)
        {
            if (def.Value is not { SizeInBlocks: > 0 } val)
                continue;

            var sample = soundDecoder.Decode(val, data.Span[0x100000..]);
            if (sample == null)
                continue;

            sample[NumericData.Id] = def.Key + 1;
            sounds.Add(sample);
        }

        var charts = new List<Chart>();

        for (var i = 0; i < ChartOffsets.Length; i++)
        {
            var offset = ChartOffsets[i];

            if (chartHeuristic.Match(data.Slice(offset, 0x4000)) == null)
                continue;

            var events = chartDecoder
                .Decode(chunk.Data.Span.Slice(offset, 0x4000))
                .Select(chartEventConverter.ConvertToBeatmaniaPc1)
                .ToList();

            if (events.Count < 1)
                continue;

            var chart = pc1ChartDecoder.Decode(events, TwinkleConstants.BeatmaniaRate);
            chart[NumericData.ByteOffset] = offset;
            chart[NumericData.Id] = i;
            chartMetadataDecoder.AddMetadata(chart, i);

            charts.Add(chart);
        }

        if (!options.DoNotConsolidateSamples)
            soundConsolidator.Consolidate(sounds, charts);

        return new TwinkleArchive
        {
            Charts = charts,
            Samples = sounds
        };
    }

    public BeatmaniaPcSongSet MigrateToBemaniPc(TwinkleBeatmaniaChunk chunk)
    {
        if (chunk.Data.Length < TwinkleConstants.ChunkSize)
            return new BeatmaniaPcSongSet();

        var data = chunk.Data[..TwinkleConstants.ChunkSize];

        var definitions = new Dictionary<int, TwinkleBeatmaniaSoundDefinition?>(256);

        for (var i = 0; i < 255; i++)
        {
            var def = soundDefinitionDecoder.Decode(data.Span[(i * 0x12)..]);
            definitions.Add(i, def);
        }

        var sounds = new List<BeatmaniaPcAudioEntry>(definitions.Count);

        foreach (var def in definitions)
        {
            var source = def.Value != null
                ? soundDecoder.Decode(def.Value, data.Span[0x100000..])
                : new Sound { Samples = [new Sample { Data = Memory<float>.Empty }], [NumericData.Rate] = 44100 };

            using var mem = new MemoryStream();
            riffStreamWriter.Write(mem, riffPcm16SoundEncoder.Encode(source));
            mem.Flush();

            sounds.Add(new BeatmaniaPcAudioEntry
            {
                Channel = def.Value?.Channel ?? 255,
                Data = mem.ToArray(),
                ExtraInfo = Memory<byte>.Empty,
                Panning = def.Value?.Panning ?? 0x40,
                Volume = def.Value?.Volume ?? 0x01
            });
        }

        var charts = new List<BeatmaniaPc1Chart>(ChartOffsets.Length);

        for (var i = 0; i < ChartOffsets.Length; i++)
        {
            var offset = ChartOffsets[i];

            if (chartHeuristic.Match(data.Slice(offset, 0x4000)) == null)
                continue;

            var events = chartDecoder
                .Decode(data.Span.Slice(offset, 0x4000))
                .Select(chartEventConverter.ConvertToBeatmaniaPc1)
                .ToList();

            if (events.Count < 1)
                continue;

            var noteCountEvents =
                chartEventConverter.ConvertNoteCountsToBeatmaniaPc1(
                    chartDecoder.GetNoteCounts(chunk.Data.Span.Slice(offset, 0x4000)));

            charts.Add(new BeatmaniaPc1Chart
            {
                Index = i,
                Data = noteCountEvents.Concat(events).ToList()
            });
        }

        return new BeatmaniaPcSongSet
        {
            Charts = charts,
            Sounds = sounds
        };
    }
}