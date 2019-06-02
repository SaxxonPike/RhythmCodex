using System;
using System.Linq;
using RhythmCodex.Beatmania.Converters;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Twinkle.Heuristics;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters
{
    [Service]
    public class TwinkleBeatmaniaDecoder : ITwinkleBeatmaniaDecoder
    {
        private readonly ITwinkleBeatmaniaSoundDefinitionDecoder _twinkleBeatmaniaSoundDefinitionDecoder;
        private readonly ITwinkleBeatmaniaSoundDecoder _twinkleBeatmaniaSoundDecoder;
        private readonly ITwinkleBeatmaniaChartDecoder _twinkleBeatmaniaChartDecoder;
        private readonly ITwinkleBeatmaniaChartEventConverter _twinkleBeatmaniaChartEventConverter;
        private readonly IBeatmaniaPc1ChartDecoder _beatmaniaPc1ChartDecoder;
        private readonly ITwinkleBeatmaniaChartMetadataDecoder _twinkleBeatmaniaChartMetadataDecoder;
        private readonly ITwinkleBeatmaniaChartHeuristic _twinkleBeatmaniaChartHeuristic;

        public TwinkleBeatmaniaDecoder(
            ITwinkleBeatmaniaSoundDefinitionDecoder twinkleBeatmaniaSoundDefinitionDecoder,
            ITwinkleBeatmaniaSoundDecoder twinkleBeatmaniaSoundDecoder,
            ITwinkleBeatmaniaChartDecoder twinkleBeatmaniaChartDecoder,
            ITwinkleBeatmaniaChartEventConverter twinkleBeatmaniaChartEventConverter,
            IBeatmaniaPc1ChartDecoder beatmaniaPc1ChartDecoder,
            ITwinkleBeatmaniaChartMetadataDecoder twinkleBeatmaniaChartMetadataDecoder,
            ITwinkleBeatmaniaChartHeuristic twinkleBeatmaniaChartHeuristic
            )
        {
            _twinkleBeatmaniaSoundDefinitionDecoder = twinkleBeatmaniaSoundDefinitionDecoder;
            _twinkleBeatmaniaSoundDecoder = twinkleBeatmaniaSoundDecoder;
            _twinkleBeatmaniaChartDecoder = twinkleBeatmaniaChartDecoder;
            _twinkleBeatmaniaChartEventConverter = twinkleBeatmaniaChartEventConverter;
            _beatmaniaPc1ChartDecoder = beatmaniaPc1ChartDecoder;
            _twinkleBeatmaniaChartMetadataDecoder = twinkleBeatmaniaChartMetadataDecoder;
            _twinkleBeatmaniaChartHeuristic = twinkleBeatmaniaChartHeuristic;
        }

        private static readonly int[] ChartOffsets = Enumerable
            .Range(0, 0x3F)
            .Select(i => i * 0x4000 + 0x2000)
            .ToArray();

        public TwinkleArchive Decode(TwinkleBeatmaniaChunk chunk)
        {
            if (chunk?.Data == null || chunk.Data.Length < 0x1A00000)
                return null;
            
            var definitions = Enumerable.Range(0, 255)
                .ToDictionary(i => i,
                    i => _twinkleBeatmaniaSoundDefinitionDecoder.Decode(chunk.Data.AsSpan(i * 0x12)));

            var sounds = definitions
                .Where(def => def.Value != null && def.Value.SizeInBlocks > 0)
                .Select(def =>
                {
                    var sample = _twinkleBeatmaniaSoundDecoder.Decode(def.Value, chunk.Data.AsSpan(0x100000));
                    sample[NumericData.Id] = def.Key + 1;
                    return sample;
                })
                .ToList();

            var charts = ChartOffsets
                .Select((offset, index) =>
                {
                    if (_twinkleBeatmaniaChartHeuristic.Match(chunk.Data.AsSpan(offset, 0x4000)) == null)
                        return null;
                    
                    var events = _twinkleBeatmaniaChartDecoder
                        .Decode(chunk.Data.AsSpan(offset), 0x4000)
                        .Select(_twinkleBeatmaniaChartEventConverter.ConvertToBeatmaniaPc1)
                        .ToList();

                    if (!events.Any())
                        return null;
                    
                    var chart = _beatmaniaPc1ChartDecoder.Decode(events, TwinkleConstants.BeatmaniaRate);
                    if (chart != null)
                    {
                        chart[NumericData.ByteOffset] = offset;
                        chart[NumericData.Id] = index;
                        _twinkleBeatmaniaChartMetadataDecoder.AddMetadata(chart, index);
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
    }
}