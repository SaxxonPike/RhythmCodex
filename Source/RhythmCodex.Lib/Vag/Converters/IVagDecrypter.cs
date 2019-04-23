using System;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters
{
    public interface IVagDecrypter
    {
        void Decrypt(ReadOnlySpan<byte> input, Span<float> output, int length, VagDecodeState decodeState);
    }
}