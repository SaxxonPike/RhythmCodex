using System;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters
{
    public interface IMicrosoftAdpcmDecoder
    {
        ISound Decode(ReadOnlySpan<byte> data, IWaveFormat fmtChunk, MsAdpcmFormat msAdpcmFormat);
    }
}