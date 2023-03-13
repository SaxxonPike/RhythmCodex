using System.IO;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.ThirdParty;

public interface IOggDecoder
{
    ISound Decode(Stream stream);
}