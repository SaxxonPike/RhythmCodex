using System.IO;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Ogg.Converters;

public interface IOggDecoder
{
    Sound? Decode(Stream stream);
}