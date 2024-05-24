using System;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters;

public interface IMicrosoftAdpcmDecoder
{
    Sound? Decode(ReadOnlySpan<byte> data, IWaveFormat fmtChunk, MicrosoftAdpcmFormat microsoftAdpcmFormat);
}