using System;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Mixer.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Mixer.Converters;

public abstract class StereoMixer : IStereoMixer
{
    public virtual (MixState State, int Mixed) Mix(
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
            return (state, 0);

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
            AudioSimd.MixGain(outRight, 1, rightData,
                (float)(balance.Sample.FromRight.ToLeft * balance.Master.FromRight.ToLeft));

            AudioSimd.MixGain(outRight, 1, rightData,
                (float)(balance.Sample.FromRight.ToRight * balance.Master.FromRight.ToRight));
        }

        var mixed = Math.Max(leftMixSize, rightMixSize);

        return (state with
        {
            Balance = balance,
            SampleOffset = state.SampleOffset + mixed
        }, mixed);
    }

    public virtual Sound? MixDown(Sound? sound, Metadata? metadata)
    {
        if (sound == null)
            return null;

        Span<float> left = stackalloc float[8192];
        Span<float> right = stackalloc float[8192];

        var result = new SoundBuilder(2);

        result.CloneMetadataFrom(sound);
        
        var state = new MixState
        {
            Sound = sound,
            EventData = metadata
        };

        while (true)
        {
            var (nextState, mixed) = Mix(left, right, state);
            if (mixed < 1)
                break;
            result.Samples[0].Append(left[..mixed]);
            result.Samples[1].Append(right[..mixed]);
            left.Clear();
            right.Clear();
            state = nextState;
        }

        if (sound.Samples.Count == 1)
        {
            result.Samples[0].CloneMetadataFrom(sound.Samples[0]);
            result.Samples[1].CloneMetadataFrom(sound.Samples[0]);
        }
        else if (sound.Samples.Count >= 2)
        {
            result.Samples[0].CloneMetadataFrom(sound.Samples[0]);
            result.Samples[1].CloneMetadataFrom(sound.Samples[1]);
        }

        result[NumericData.Panning] = null;
        result[NumericData.Volume] = null;
        result.Samples[0][NumericData.Panning] = null;
        result.Samples[0][NumericData.Volume] = null;
        result.Samples[1][NumericData.Panning] = null;
        result.Samples[1][NumericData.Volume] = null;

        return result.ToSound();
    }

    protected abstract MixAmount GetSampleMix(Sample? left, Sample? right, MixState state);

    protected abstract MixAmount GetMasterMix(MixState state);
}