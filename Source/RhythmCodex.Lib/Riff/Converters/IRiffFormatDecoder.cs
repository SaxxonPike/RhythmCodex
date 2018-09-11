using RhythmCodex.Riff.Models;

namespace RhythmCodex.Riff.Converters
{
    public interface IRiffFormatDecoder
    {
        IRiffFormat Decode(IRiffChunk chunk);
    }
}