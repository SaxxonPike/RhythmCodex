using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters
{
    [Service]
    public class TwinkleBeatmaniaDecoder : ITwinkleBeatmaniaDecoder
    {
        private readonly ITwinkleBeatmaniaSoundDefinitionDecoder _twinkleBeatmaniaSoundDefinitionDecoder;
        private readonly ITwinkleBeatmaniaSoundDecoder _twinkleBeatmaniaSoundDecoder;

        public TwinkleBeatmaniaDecoder(
            ITwinkleBeatmaniaSoundDefinitionDecoder twinkleBeatmaniaSoundDefinitionDecoder,
            ITwinkleBeatmaniaSoundDecoder twinkleBeatmaniaSoundDecoder
            )
        {
            _twinkleBeatmaniaSoundDefinitionDecoder = twinkleBeatmaniaSoundDefinitionDecoder;
            _twinkleBeatmaniaSoundDecoder = twinkleBeatmaniaSoundDecoder;
        }
        
        public TwinkleArchive Decode(TwinkleBeatmaniaChunk chunk)
        {
            var definitions = Enumerable.Range(0, 256)
                .ToDictionary(i => i,
                    i => _twinkleBeatmaniaSoundDefinitionDecoder.Decode(chunk.Data.AsSpan(i * 0x12)));
            
            return new TwinkleArchive
            {
                Charts = new List<IChart>(),
                Samples = definitions
                    .Where(def => def.Value.SizeInBlocks > 0)
                    .Select(def =>
                    {
                        var sample = _twinkleBeatmaniaSoundDecoder.Decode(def.Value, chunk.Data.AsSpan(0x100000));
                        sample[NumericData.Id] = def.Key;
                        return sample;
                    })
                    .ToList()
            };
        }
    }
}