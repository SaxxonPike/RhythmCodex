using RhythmCodex.Audio.Models;

namespace RhythmCodex.Audio.Converters
{
    public interface IRiffFormatDecoder
    {
        IRiffFormat Decode(IRiffChunk chunk);
    }
}