using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Converters
{
    [Service]
    public class TwinkleBeatmaniaSoundDefinitionDecoder : ITwinkleBeatmaniaSoundDefinitionDecoder
    {
        public TwinkleBeatmaniaSoundDefinition Decode(ReadOnlySpan<byte> data)
        {
            if (data.Length < 0x12)
                return null;
            
            var invalid = true;
            for (var i = 1; i < 0x12; i++)
            {
                if (data[i] != data[0])
                {
                    invalid = false;
                    break;
                }
            }

            if (invalid)
                return null;
            
            return new TwinkleBeatmaniaSoundDefinition
            {
                Channel = data[0x00],
                Flags01 = data[0x01],
                Frequency = Bitter.ToInt16S(data, 0x02) & 0xFFFF,
                Volume = data[0x04],
                Panning = data[0x05],
                SampleStart = Bitter.ToInt24S(data, 0x06) & 0xFFFFFF,
                SampleEnd = Bitter.ToInt24S(data, 0x09) & 0xFFFFFF,
                Value0C = Bitter.ToInt16S(data, 0x0C) & 0xFFFF,
                Flags0E = data[0x0E],
                Flags0F = data[0x0F],
                SizeInBlocks = Bitter.ToInt16S(data, 0x10) & 0xFFFF
            };
        }
    }
}