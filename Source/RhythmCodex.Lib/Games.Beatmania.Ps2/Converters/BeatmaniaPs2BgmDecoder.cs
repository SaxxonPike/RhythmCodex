using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Converters;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

[Service]
public class BeatmaniaPs2BgmDecoder(
    IVagDecoder vagDecoder,
    IBeatmaniaPs2Mixer mixer)
    : IBeatmaniaPs2BgmDecoder
{
    public Sound? Decode(BeatmaniaPs2Bgm bgm)
    {
        var output = vagDecoder.Decode(bgm.Data);
        if (output == null)
            return null;

        output.Skip(56);
        output[NumericData.Rate] = bgm.Rate;
        output[NumericData.SourceVolume] = bgm.Volume;
        output[NumericData.Volume] = new BigRational(bgm.Volume, bgm.VolumeScale);
        output[NumericData.Panning] = BigRational.OneHalf;
        output.Mixer = () => mixer;
        return output;
    }
}