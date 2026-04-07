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
        if (bgm.Data == null)
            return null;
        
        var output = vagDecoder.Decode(bgm.Data);

        if (bgm.Skip > 0)
            output.Skip(bgm.Skip);

        output[NumericData.Rate] = bgm.Rate;
        output[NumericData.SourceVolume] = bgm.Volume;
        output[NumericData.Volume] = new BigRational(bgm.Volume, bgm.VolumeScale);
        output[NumericData.Panning] = BigRational.OneHalf;
        output[NumericData.Id] = bgm.Index;
        output.Mixer = () => mixer;
        return output;
    }
}