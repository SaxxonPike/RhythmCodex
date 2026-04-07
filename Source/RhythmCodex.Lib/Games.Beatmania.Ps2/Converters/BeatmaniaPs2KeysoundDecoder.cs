using System.Linq;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Resampler.Resamplers;
using RhythmCodex.Sounds.Vag.Converters;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

/// <inheritdoc />
[Service]
public class BeatmaniaPs2KeysoundDecoder(IVagDecoder vagDecoder, IBeatmaniaPs2Mixer mixer,
    IPsxGaussianResampler resampler)
    : IBeatmaniaPs2KeysoundDecoder
{
    /// <inheritdoc />
    public Sound Decode(BeatmaniaPs2Keysound keysound)
    {
        const int targetFrequency = 44100;

        var samples = keysound.Data
            .SelectMany(d => vagDecoder.Decode(d).Samples)
            .ToList();

        var leftRate = keysound.FrequencyLeft == 0
            ? null
            : (int?)keysound.FrequencyLeft;

        var rightRate = keysound.FrequencyRight == 0
            ? null
            : (int?)keysound.FrequencyRight;

        //
        // Perform resampling.
        //

        if (samples.Count >= 1 && leftRate is not null and not targetFrequency)
        {
            samples[0] = samples[0].CloneWithData(resampler.Resample(samples[0].Data.Span, leftRate.Value, targetFrequency));
            leftRate = targetFrequency;
        }

        if (samples.Count >= 2 && rightRate is not null and not targetFrequency)
        {
            samples[1] = samples[1].CloneWithData(resampler.Resample(samples[1].Data.Span, rightRate.Value, targetFrequency));
            rightRate = targetFrequency;
        }

        //
        // Double mono sounds to stereo.
        //

        if (samples.Count == 1)
        {
            samples.Add(samples[0].Clone());
            rightRate = leftRate;
        }

        //
        // Set sample metadata.
        //

        for (var i = 0; i < samples.Count; i++)
        {
            samples[i][NumericData.Volume] = new BigRational(keysound.Volume, keysound.VolumeScale);
            samples[i][NumericData.SourceVolume] = keysound.Volume;

            samples[i][NumericData.Panning] = new BigRational(
                (i & 1) == 0 ? keysound.PanningLeft : keysound.PanningRight, keysound.PanningScale
            );

            samples[i][NumericData.SourcePanning] =
                (i & 1) == 0 ? keysound.PanningLeft : keysound.PanningRight;

            samples[i][NumericData.SourceRate] = (i & 1) == 0
                ? keysound.FrequencyLeft
                : keysound.FrequencyRight;

            samples[i][NumericData.Rate] = (i & 1) == 0
                ? leftRate
                : rightRate;
        }

        return new Sound
        {
            Samples = samples,
            [NumericData.Channel] = keysound.Channel,
            Mixer = () => mixer
        };
    }
}