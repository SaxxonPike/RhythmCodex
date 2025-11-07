using System;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Wav.Converters;

public interface IMicrosoftAdpcmEncoder
{
    Memory<byte> Encode(Sound sound, int samplesPerBlock);
    int GetBlockSize(int samplesPerBlock, int channels);
}