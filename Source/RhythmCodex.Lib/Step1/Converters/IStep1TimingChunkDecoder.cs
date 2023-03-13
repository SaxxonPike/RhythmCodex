using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Step1.Converters;

public interface IStep1TimingChunkDecoder
{
    TimingChunk Convert(byte[] data);
}