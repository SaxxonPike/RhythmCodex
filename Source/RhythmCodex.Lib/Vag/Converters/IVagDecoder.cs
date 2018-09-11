using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters
{
    public interface IVagDecoder
    {
        ISound Decode(VagChunk chunk);
    }
}