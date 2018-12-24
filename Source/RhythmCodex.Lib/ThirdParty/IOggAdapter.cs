using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.ThirdParty
{
    public interface IOggAdapter
    {
        ISound Decode(Stream stream);
    }
}