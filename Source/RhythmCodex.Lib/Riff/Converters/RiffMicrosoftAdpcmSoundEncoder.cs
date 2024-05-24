using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Wav.Converters;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Riff.Converters;

[Service]
public class RiffMicrosoftAdpcmSoundEncoder(
    IRiffFormatEncoder riffFormatEncoder,
    IMicrosoftAdpcmEncoder microsoftAdpcmEncoder)
    : IRiffMicrosoftAdpcmSoundEncoder
{
    public IRiffContainer Encode(Sound? sound, int samplesPerBlock)
    {
        var sampleRate = sound[NumericData.Rate];

        if (sampleRate == null)
        {
            var sampleRates = sound
                .Samples
                .Select(s => s[NumericData.Rate])
                .Where(r => r != null)
                .Distinct()
                .ToArray();
            sampleRate = sampleRates.SingleOrDefault();
        }

        if (sampleRate == null)
            sampleRate = 44100;

        var channels = sound.Samples.Count;
        var byteRate = sampleRate * channels * 2 / 4;  //not accurate but not far off
            
        var container = new RiffContainer
        {
            Format = "WAVE",
            Chunks = []
        };
            
        var extraFormat = new MicrosoftAdpcmFormat
        {
            Coefficients = MicrosoftAdpcmConstants.DefaultCoefficients,
            SamplesPerBlock = 500
        };
            
        var format = new RiffFormat
        {
            Format = 2,
            SampleRate = (int) sampleRate,
            Channels = channels,
            ByteRate = (int) byteRate,
            BitsPerSample = 4,
            BlockAlign = microsoftAdpcmEncoder.GetBlockSize(samplesPerBlock, channels),
            ExtraData = extraFormat.ToBytes()
        };

        container.Chunks.Add(riffFormatEncoder.Encode(format));
        container.Chunks.Add(new RiffChunk
        {
            Id = "data",
            Data = microsoftAdpcmEncoder.Encode(sound, samplesPerBlock)
        });
            
        return container;
    }
}