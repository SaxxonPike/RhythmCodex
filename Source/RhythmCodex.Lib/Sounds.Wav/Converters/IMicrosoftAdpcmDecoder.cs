using System;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Wav.Models;

namespace RhythmCodex.Sounds.Wav.Converters;

public interface IMicrosoftAdpcmDecoder
{
    Sound? Decode(ReadOnlySpan<byte> data, IWaveFormat fmtChunk, MicrosoftAdpcmFormat microsoftAdpcmFormat);
}