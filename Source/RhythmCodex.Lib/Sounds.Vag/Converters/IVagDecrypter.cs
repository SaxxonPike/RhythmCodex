using System;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Sounds.Vag.Converters;

public interface IVagDecrypter
{
    int Decrypt(ReadOnlySpan<byte> input, Span<float> output, int length, VagState state);
}