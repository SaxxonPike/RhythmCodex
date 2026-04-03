using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Mixer.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Mixer.Converters;

[Service]
public class DefaultLinearMixer : StereoMixer, IDefaultLinearMixer
{
        protected override MixAmount GetSampleMix(Sample? left, Sample? right, MixState state)
    {
        var leftPanning = (double)(left?[NumericData.Panning] ?? BigRational.Zero);
        var rightPanning = (double)(right?[NumericData.Panning] ?? BigRational.One);

        var leftVolume = (double)(left?[NumericData.Volume] ?? BigRational.One);
        var rightVolume = (double)(right?[NumericData.Volume] ?? BigRational.One);

        return new MixAmount(
            FromLeft: (
                ToLeft: (1 - leftPanning) * leftVolume,
                ToRight: leftPanning * leftVolume
            ),
            FromRight: (
                ToLeft: (1 - rightPanning) * rightVolume,
                ToRight: rightPanning * rightVolume
            )
        );
    }

    protected override MixAmount GetMasterMix(MixState state)
    {
        var volume = (double)(
            state.EventData?[NumericData.Volume] ??
            state.Sound?[NumericData.Volume] ??
            BigRational.One
        );

        var panning = (double)(
            state.EventData?[NumericData.Panning] ??
            state.Sound?[NumericData.Panning] ??
            BigRational.OneHalf
        );

        // Range -2..2
        var polarizedPanning = (panning - 0.5d) * 4;

        var leftLeft = Math.Clamp(2d - polarizedPanning, 0, 1);
        var leftRight = Math.Clamp(polarizedPanning, 0, 1);

        var rightLeft = Math.Clamp(-polarizedPanning, 0, 1);
        var rightRight = Math.Clamp(2d + polarizedPanning, 0, 1);

        return new MixAmount(
            FromLeft: (
                ToLeft: leftLeft * volume,
                ToRight: leftRight * volume
            ),
            FromRight: (
                ToLeft: rightLeft * volume,
                ToRight: rightRight * volume
            )
        );
    }
}