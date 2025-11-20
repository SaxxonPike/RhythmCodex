using System;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Riff.Models;
using RhythmCodex.Sounds.Wav.Converters;
using RhythmCodex.Sounds.Wav.Models;

namespace RhythmCodex.Sounds.Riff.Converters;

[Service]
public class RiffMicrosoftAdpcmSoundEncoder(
    IRiffFormatEncoder riffFormatEncoder,
    IMicrosoftAdpcmEncoder microsoftAdpcmEncoder)
    : IRiffMicrosoftAdpcmSoundEncoder
{
    public RiffContainer Encode(Sound sound, int samplesPerBlock)
    {
        ArgumentNullException.ThrowIfNull(sound, nameof(sound));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(samplesPerBlock, nameof(samplesPerBlock));
        
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
            SamplesPerBlock = 500
        };
            
        var format = new RiffFormat
        {
            Format = 2,
            SampleRate = (int) sampleRate,
            Channels = channels,
            ByteRate = (int) byteRate!,
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