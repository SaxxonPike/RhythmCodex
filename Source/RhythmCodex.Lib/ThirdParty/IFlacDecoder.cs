using System.IO;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.ThirdParty
{
    public interface IFlacDecoder
    {
        ISound Decode(Stream stream);
    }
}