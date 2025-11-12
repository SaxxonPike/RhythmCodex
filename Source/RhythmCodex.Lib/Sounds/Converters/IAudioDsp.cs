using System;
using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Resampler.Providers;

namespace RhythmCodex.Sounds.Converters;

public interface IAudioDsp
{
    Sound ApplyEffects(Sound sound);
    Sound ApplyResampling(Sound sound, IResampler resampler, BigRational rate);
    Sound? Normalize(Sound sound, BigRational target, bool cutOnly);
    Sound IntegerDownsample(Sound sound, int factor);
    Sound Mix(IEnumerable<Sound> sound);
    byte[] Interleave16Bits(Sound sound);
    Sample[] BytesToSamples(ReadOnlySpan<byte> data, int bitsPerSample, int channels, bool bigEndian);
    Sample[] FloatsToSamples(ReadOnlySpan<float> data, int channels);
    (float[] A, float[] B) Deinterleave2(Span<float> data);
}