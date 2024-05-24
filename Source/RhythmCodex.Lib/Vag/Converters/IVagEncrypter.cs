using System;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters;

public interface IVagEncrypter
{
    void Encrypt(ReadOnlySpan<float> input, Span<byte> output, int length, VagState state);
}