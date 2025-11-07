using RhythmCodex.Sounds.Riff.Models;
using RhythmCodex.Sounds.Wav.Models;

namespace RhythmCodex.Sounds.Wav.Converters;

public interface IWaveFmtDecoder
{
    WaveFmtChunk Decode(RiffChunk chunk);
}