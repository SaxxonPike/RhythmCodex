using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.ThirdParty
{
    public interface IOggDecoder
    {
        ISound Decode(Stream stream);
    }
}