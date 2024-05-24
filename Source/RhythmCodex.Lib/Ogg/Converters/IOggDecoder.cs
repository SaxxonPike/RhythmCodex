using System.IO;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Ogg.Converters;

public interface IOggDecoder
{
    Sound? Decode(Stream stream);
}