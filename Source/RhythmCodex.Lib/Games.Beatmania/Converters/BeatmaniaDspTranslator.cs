using System;
using RhythmCodex.Archs.Djmain;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Converters;

namespace RhythmCodex.Games.Beatmania.Converters;

[Service]
public class BeatmaniaDspTranslator : IBeatmaniaDspTranslator
{
    public BigRational GetTwinkleVolume(int volume)
    {
        return Math.Pow(1d / Decibels.ToFactor(4.5d / 16d), volume);
    }
    
    public BigRational GetFirebeatVolume(int volume)
    {
        const double referenceGain = -36.0d;
        const double referenceValue = 0x90;

        return new BigRational(Decibels.ToFactor(referenceGain * volume / referenceValue));
    }
        
    public BigRational GetLinearVolume(int volume)
    {
        if (volume > 0x7F)
            volume = 0x7F;
        return new BigRational(volume, 0x7F);
    }

    public BigRational GetDjmainVolume(int volume)
    {
        return volume switch
        {
            < 0 => 1,
            > 0x7E => 0,
            _ => DjmainConstants.VolumeRom.Span[volume] / (double)0x7FFF
        };
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

        // Djmain swaps its stereo channels.
        return new BigRational(0xE - (panning - 1), 0xE);
    }

    public BigRational GetDjmainRate(int rate)
    {
        return DjmainConstants.SampleRateMultiplier * rate;
    }
}