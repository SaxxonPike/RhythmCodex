using System.Linq;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Converters;

namespace RhythmCodex.Beatmania.Converters;

[Service]
public class BeatmaniaPs2KeysoundDecoder(IVagDecoder vagDecoder, IBeatmaniaDspTranslator beatmaniaDspTranslator)
    : IBeatmaniaPs2KeysoundDecoder
{
    public Sound? Decode(BeatmaniaPs2Keysound keysound)
    {
        var samples = keysound.Data.SelectMany(d => vagDecoder.Decode(d).Samples).ToList();
        var leftRate = keysound.FrequencyLeft == 0 ? null : (int?)keysound.FrequencyLeft;
        var rightRate = keysound.FrequencyRight == 0 ? null : (int?)keysound.FrequencyRight;
        var left = true;
        foreach (var sample in samples)
        {
            sample[NumericData.Rate] = left ? leftRate : rightRate;
            left = !left;
        }
            
        return new Sound
        {
            Samples = samples,
            [NumericData.Volume] = beatmaniaDspTranslator.GetLinearVolume(keysound.Volume),
            [NumericData.SourceVolume] = keysound.Volume,
            [NumericData.Panning] = beatmaniaDspTranslator.GetBm2dxPanning(keysound.Panning),
            [NumericData.SourcePanning] = keysound.Panning,
            [NumericData.Channel] = keysound.Channel
        };
    }
}