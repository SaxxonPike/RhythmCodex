using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.ThirdParty
{
    public interface IFlacDecoder
    {
        ISound Decode(Stream stream);
    }
}