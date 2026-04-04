using System.Linq;
using RhythmCodex.Games.Beatmania.Converters;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Mixer.Converters;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Converters;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

[Service]
public class BeatmaniaPs2KeysoundDecoder(IVagDecoder vagDecoder, IBeatmaniaDspTranslator beatmaniaDspTranslator,
    IBeatmaniaPs2Mixer mixer)
    : IBeatmaniaPs2KeysoundDecoder
{
    public Sound Decode(BeatmaniaPs2Keysound keysound)
    {
        var samples = keysound.Data
            .SelectMany(d => vagDecoder.Decode(d)?.Samples ?? [])
            .ToList();

        if (samples.Count == 1)
        {
            samples.Add(samples[0].Clone());
        }

        var leftRate = keysound.FrequencyLeft == 0
            ? null
            : (int?)keysound.FrequencyLeft;

        var rightRate = keysound.FrequencyRight == 0
            ? null
            : (int?)keysound.FrequencyRight;

        for (var i = 0; i < samples.Count; i++)
        {
            samples[i][NumericData.Volume] = new BigRational(keysound.Volume, 127);
            samples[i][NumericData.SourceVolume] = keysound.Volume;

            samples[i][NumericData.Panning] = new BigRational(
                (i & 1) == 0 ? keysound.PanningLeft : keysound.PanningRight, 127
            );

            samples[i][NumericData.SourcePanning] =
                (i & 1) == 0 ? keysound.PanningLeft : keysound.PanningRight;

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