using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Wav.Converters;

public interface IMicrosoftAdpcmEncoder
{
    byte[] Encode(ISound sound, int samplesPerBlock);
    int GetBlockSize(int samplesPerBlock, int channels);
}