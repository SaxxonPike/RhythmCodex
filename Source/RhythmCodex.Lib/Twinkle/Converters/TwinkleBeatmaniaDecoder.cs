using System;
using System.Linq;
using RhythmCodex.Beatmania.Converters;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Processing;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters
{
    [Service]
    public class TwinkleBeatmaniaDecoder : ITwinkleBeatmaniaDecoder
    {
        private readonly ITwinkleBeatmaniaSoundDefinitionDecoder _twinkleBeatmaniaSoundDefinitionDecoder;
        private readonly ITwinkleBeatmaniaSoundDecoder _twinkleBeatmaniaSoundDecoder;
        private readonly ISoundConsolidator _soundConsolidator;
        private readonly ITwinkleBeatmaniaChartDecoder _twinkleBeatmaniaChartDecoder;
        private readonly ITwinkleBeatmaniaChartEventConverter _twinkleBeatmaniaChartEventConverter;
        private readonly IBeatmaniaPc1ChartDecoder _beatmaniaPc1ChartDecoder;

        public TwinkleBeatmaniaDecoder(
            ITwinkleBeatmaniaSoundDefinitionDecoder twinkleBeatmaniaSoundDefinitionDecoder,
            ITwinkleBeatmaniaSoundDecoder twinkleBeatmaniaSoundDecoder,
            ISoundConsolidator soundConsolidator,
            ITwinkleBeatmaniaChartDecoder twinkleBeatmaniaChartDecoder,
            ITwinkleBeatmaniaChartEventConverter twinkleBeatmaniaChartEventConverter,
            IBeatmaniaPc1ChartDecoder beatmaniaPc1ChartDecoder
            )
        {
            _twinkleBeatmaniaSoundDefinitionDecoder = twinkleBeatmaniaSoundDefinitionDecoder;
            _twinkleBeatmaniaSoundDecoder = twinkleBeatmaniaSoundDecoder;
            _soundConsolidator = soundConsolidator;
            _twinkleBeatmaniaChartDecoder = twinkleBeatmaniaChartDecoder;
            _twinkleBeatmaniaChartEventConverter = twinkleBeatmaniaChartEventConverter;
            _beatmaniaPc1ChartDecoder = beatmaniaPc1ChartDecoder;
        }

        private readonly int[] ChartOffsets = {
            0x0002000,
            0x0006000,
            0x000A000,
            0x000E000,
            0x0012000,
            0x0016000,
            0x001A000,
            0x001E000
        };

        private readonly string[] Difficulties = {
            "5key",
            "light",
            "normal",
            "another",
            "12000",
            "16000",
            "1A000",
            "1E000"
        };
        
        public TwinkleArchive Decode(TwinkleBeatmaniaChunk chunk)
        {
            var definitions = Enumerable.Range(0, 255)
                .ToDictionary(i => i,
                    i => _twinkleBeatmaniaSoundDefinitionDecoder.Decode(chunk.Data.AsSpan(i * 0x12)));

            var sounds = definitions
                .Where(def => def.Value.SizeInBlocks > 0)
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
                    var events = _twinkleBeatmaniaChartDecoder
                        .Decode(chunk.Data.AsSpan(offset), 0x4000)
                        .Select(_twinkleBeatmaniaChartEventConverter.ConvertToBeatmaniaPc1)
                        .ToList();

                    if (!events.Any())
                        return null;
                    
                    var chart = _beatmaniaPc1ChartDecoder.Decode(events, new BigRational(598, 10));
                    if (chart != null)
                    {
                        chart[NumericData.ByteOffset] = offset;
                        chart[NumericData.Id] = index;
                        chart[StringData.Difficulty] = Difficulties[index];
                    }

                    return chart;
                })
                .Where(c => c != null)
                .ToList();

            _soundConsolidator.Consolidate(sounds, charts.SelectMany(c => c.Events));
            
            return new TwinkleArchive
            {
                Charts = charts,
                Samples = sounds
            };
        }
    }
}