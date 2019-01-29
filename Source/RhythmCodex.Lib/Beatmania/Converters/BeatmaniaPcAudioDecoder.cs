using System;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;
using RhythmCodex.Wav.Converters;

namespace RhythmCodex.Beatmania.Converters
{
    [Service]
    public class BeatmaniaPcAudioDecoder : IBeatmaniaPcAudioDecoder
    {
        private readonly IWavDecoder _wavDecoder;

        public BeatmaniaPcAudioDecoder(IWavDecoder wavDecoder)
        {
            _wavDecoder = wavDecoder;
        }

        private static readonly BigRational[] VolumeTable =
            Enumerable
                .Range(0, 256)
                .Select(i => new BigRational(Math.Pow(10.0f, -36.0f * i / 64f / 20.0f)))
                .ToArray();

        public ISound Decode(BeatmaniaPcAudioEntry entry)
        {
            using (var wavDataMem = new MemoryStream(entry.Data))
            {
                var result = _wavDecoder.Decode(wavDataMem);

                var panning = entry.Panning;
                if (panning > 0x7F || panning < 0x01)
                    panning = 0x40;

                var volume = entry.Volume;
                if (volume < 0x01)
                    volume = 0x01;
                else if (volume > 0xFF)
                    volume = 0xFF;

                result[NumericData.Panning] = (panning - 1.0d) / 126.0d;
                result[NumericData.Volume] = VolumeTable[volume];
                result[NumericData.Channel] = entry.Channel;
                result[NumericData.SourceVolume] = volume;
                result[NumericData.SourcePanning] = panning;

                return result;
            }
        }
    }
}