using System;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Sounds.Vag.Converters;

public interface IVagEncrypter
{
    void Encrypt(ReadOnlySpan<float> input, Span<byte> output, int length, VagState state);
}