using System.IO;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Mp3.Converters;

public interface IMp3Decoder
{
    Sound? Decode(Stream stream);
}