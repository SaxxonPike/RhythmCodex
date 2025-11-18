using System;
using JetBrains.Annotations;
using RhythmCodex.Infrastructure;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Mixer.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Mixer.Converters;

[PublicAPI]
public class DefaultStereoMixer : IStereoMixer
{
    public MixState? Mix(
        Span<float> outLeft,
        Span<float> outRight,
        MixState state
    )
    {
        var leftSample = state.Sound?.Samples.Count switch
        {
            null or 0 => null,
            _ => state.Sound.Samples[0]
        };

        var rightSample = state.Sound?.Samples.Count switch
        {
            null or 0 => null,
            1 => state.Sound.Samples[0],
            _ => state.Sound.Samples[1]
        };

        var leftData = leftSample == null || state.SampleOffset >= leftSample.Data.Length
            ? ReadOnlySpan<float>.Empty
            : leftSample.Data.Span[state.SampleOffset..];

        var rightData = rightSample == null || state.SampleOffset >= rightSample.Data.Length
            ? ReadOnlySpan<float>.Empty
            : rightSample.Data.Span[state.SampleOffset..];

        if (leftData.Length == 0 && rightData.Length == 0)
            return null;

        var balance = state.Balance ?? new MixBalance(
            Sample: GetSampleMix(leftSample, rightSample, state),
            Master: GetMasterMix(state)
        );

        var masterMixSize = Math.Min(outLeft.Length, outRight.Length);
        var leftMixSize = Math.Min(masterMixSize, leftData.Length);
        var rightMixSize = Math.Min(masterMixSize, rightData.Length);

        if (leftMixSize > 0)
        {
            AudioSimd.MixGain(outLeft, 1, leftData,
                (float)(balance.Sample.FromLeft.ToLeft * balance.Master.FromLeft.ToLeft));

            AudioSimd.MixGain(outLeft, 1, leftData,
                (float)(balance.Sample.FromLeft.ToRight * balance.Master.FromLeft.ToRight));
        }

        if (rightMixSize > 0)
        {
            AudioSimd.MixGain(outLeft, 1, rightData,
                (float)(balance.Sample.FromRight.ToLeft * balance.Master.FromRight.ToLeft));

            AudioSimd.MixGain(outLeft, 1, rightData,
                (float)(balance.Sample.FromRight.ToRight * balance.Master.FromRight.ToRight));
        }

        return state with
        {
            Balance = balance,
            SampleOffset = state.SampleOffset + Math.Max(leftMixSize, rightMixSize)
        };
    }

    protected virtual MixAmount GetSampleMix(Sample? left, Sample? right, MixState state)
    {
        var leftPanning = (double)(left?[NumericData.Panning] ?? BigRational.Zero);
        var rightPanning = (double)(right?[NumericData.Panning] ?? BigRational.One);

        var leftVolume = (double)(left?[NumericData.Volume] ?? BigRational.One);
        var rightVolume = (double)(right?[NumericData.Volume] ?? BigRational.One);

        return new MixAmount(
            FromLeft: (
                ToLeft: Math.Sqrt(1 - leftPanning) * leftVolume,
                ToRight: Math.Sqrt(leftPanning) * leftVolume
            ),
            FromRight: (
                ToLeft: Math.Sqrt(1 - rightPanning) * rightVolume,
                ToRight: Math.Sqrt(rightPanning) * rightVolume
            )
        );
    }

    protected virtual MixAmount GetMasterMix(MixState state)
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

        //  X|   -2     2|  -2      2|  -2      2|  -2      2|
        // Y |           |           |           |           |
        // 1 | LL --.    |           | RL        |       .-- | 1
        //   |       \   |         / |    \      |      /    |
        //   |        \  |        /  |     \     |     /     |
        //   |         \ |       /   |      \    |    /      |
        // 0 |           | LR --`    |       `-- | RR        | 0
        //   |           |           |           |           |

        // Range -2..2
        var polarizedPanning = (panning - 0.5d) * 4;

        var leftLeft = Math.Clamp(2d - polarizedPanning, 0, 1);
        var leftRight = Math.Clamp(polarizedPanning, 0, 1);

        var rightLeft = Math.Clamp(-polarizedPanning, 0, 1);
        var rightRight = Math.Clamp(2d + polarizedPanning, 0, 1);

        return new MixAmount(
            FromLeft: (
                ToLeft: Math.Sqrt(leftLeft) * volume,
                ToRight: Math.Sqrt(leftRight) * volume
            ),
            FromRight: (
                ToLeft: Math.Sqrt(rightLeft) * volume,
                ToRight: Math.Sqrt(rightRight) * volume
            )
        );
    }
}