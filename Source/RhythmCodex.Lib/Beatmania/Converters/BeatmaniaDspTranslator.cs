using System;
using RhythmCodex.Djmain;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Converters
{
    [Service]
    public class BeatmaniaDspTranslator : IBeatmaniaDspTranslator
    {
        public BigRational GetFirebeatVolume(int volume)
        {
            return new BigRational(Math.Pow(10.0f, (-36.0f * volume / 144f) / 20.0f));
        }
        
        public BigRational GetLinearVolume(int volume)
        {
            if (volume > 0x7F)
                volume = 0x7F;
            return new BigRational(volume, 0x7F);
        }

        public BigRational GetDjmainVolume(int volume)
        {
            return new BigRational(Math.Pow(10.0f, -36.0f * volume / 64f / 20.0f));
        }

        public BigRational GetBm2dxPanning(int panning)
        {
            if (panning < 0x01)
                panning = 0x01;
            if (panning > 0x7F)
                panning = 0x7F;
            return new BigRational(panning - 1, 0x7E);
        }

        public BigRational GetDjmainPanning(int panning)
        {
            panning &= 0xF;
            if (panning < 0x1)
                panning = 0x1;
            return new BigRational(panning - 1, 0xE);
        }

        public BigRational GetDjmainRate(int rate)
        {
            return DjmainConstants.SampleRateMultiplier * rate;
        }
    }
}