using RhythmCodex.Attributes;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Converters;

namespace RhythmCodex.Beatmania.Converters
{
    [Service]
    public class BeatmaniaPs2BgmDecoder : IBeatmaniaPs2BgmDecoder
    {
        private readonly IVagDecoder _vagDecoder;
        private readonly IBeatmaniaDspTranslator _beatmaniaDspTranslator;

        public BeatmaniaPs2BgmDecoder(IVagDecoder vagDecoder, IBeatmaniaDspTranslator beatmaniaDspTranslator)
        {
            _vagDecoder = vagDecoder;
            _beatmaniaDspTranslator = beatmaniaDspTranslator;
        }
        
        public ISound Decode(BeatmaniaPs2Bgm bgm)
        {
            var output = _vagDecoder.Decode(bgm.Data);
            output[NumericData.Rate] = bgm.Rate;
            output[NumericData.Channel] = bgm.Channels;
            output[NumericData.SourceVolume] = bgm.Volume;
            output[NumericData.Volume] = _beatmaniaDspTranslator.GetLinearVolume(bgm.Volume);
            output[NumericData.Panning] = BigRational.OneHalf;
            return output;
        }
    }
}