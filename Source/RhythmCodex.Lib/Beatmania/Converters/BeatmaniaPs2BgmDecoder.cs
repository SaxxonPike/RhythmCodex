using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Converters;

namespace RhythmCodex.Beatmania.Converters;

[Service]
public class BeatmaniaPs2BgmDecoder(IVagDecoder vagDecoder, IBeatmaniaDspTranslator beatmaniaDspTranslator)
    : IBeatmaniaPs2BgmDecoder
{
    public Sound? Decode(BeatmaniaPs2Bgm bgm)
    {
        var output = vagDecoder.Decode(bgm.Data);
        output[NumericData.Rate] = bgm.Rate;
        output[NumericData.Channel] = bgm.Channels;
        output[NumericData.SourceVolume] = bgm.Volume;
        output[NumericData.Volume] = beatmaniaDspTranslator.GetLinearVolume(bgm.Volume);
        output[NumericData.Panning] = BigRational.OneHalf;
        return output;
    }
}