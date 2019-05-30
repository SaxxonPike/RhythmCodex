using System;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters
{
    public interface IMicrosoftAdpcmDecoder
    {
        ISound Decode(ReadOnlySpan<byte> data, IWaveFormat fmtChunk, MicrosoftAdpcmFormat microsoftAdpcmFormat);
    }
}