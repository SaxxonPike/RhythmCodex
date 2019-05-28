using System;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters
{
    public interface IMicrosoftAdpcmEncoder
    {
        byte[] Encode(ISound sound, IWaveFormat fmtChunk, MicrosoftAdpcmFormat microsoftAdpcmFormat);
    }
}