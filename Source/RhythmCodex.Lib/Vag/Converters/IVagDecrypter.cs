using System;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters;

public interface IVagDecrypter
{
    int Decrypt(ReadOnlySpan<byte> input, Span<float> output, int length, VagState state);
}