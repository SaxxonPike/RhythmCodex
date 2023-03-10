using RhythmCodex.Riff.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters;

public interface IWaveFmtDecoder
{
    WaveFmtChunk Decode(IRiffChunk chunk);
}