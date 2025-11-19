using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Mixer.Converters;
using RhythmCodex.Sounds.Mixer.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Djmain.Converters;

[Service]
public class DjmainMixer : StereoMixer, IDjmainMixer
{
    protected override MixAmount GetMasterMix(MixState state) =>
        new((1, 0), (0, 1));

    protected override MixAmount GetSampleMix(Sample? left, Sample? right, MixState state)
    {
        var volTabIdx = Math.Clamp((int)(state.Sound?[NumericData.SourceVolume] ?? BigRational.Zero), 0x00, 0x7F);
        var panTabIdx = Math.Clamp((int)(state.Sound?[NumericData.SourcePanning] ?? 0x8), 0x1, 0xF) - 1;

        var finalVol = DjmainConstants.VolumeRom.Span[volTabIdx] / (double)0x7FFF;
        var finalPanLeft = DjmainConstants.PanRom.Span[panTabIdx] / (double)0x7FFF;
        var finalPanRight = DjmainConstants.PanRom.Span[0xE - panTabIdx] / (double)0x7FFF;

        return new MixAmount(
            FromLeft: (
                ToLeft: finalVol * finalPanLeft,
                ToRight: finalVol * (1 - finalPanLeft)
            ),
            FromRight: (
                ToLeft: finalVol * (1 - finalPanRight),
                ToRight: finalVol * finalPanRight
            )
        );
    }
}

public interface IDjmainMixer : IStereoMixer
{
}