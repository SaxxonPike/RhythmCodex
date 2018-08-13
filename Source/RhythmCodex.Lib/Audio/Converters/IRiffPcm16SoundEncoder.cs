using System.Collections.Generic;
using RhythmCodex.Audio.Models;

namespace RhythmCodex.Audio.Converters
{
    public interface IRiffPcm16SoundEncoder
    {
        IRiffContainer Encode(ISound sound);
    }
}