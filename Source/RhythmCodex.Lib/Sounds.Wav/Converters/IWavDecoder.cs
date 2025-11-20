using System.IO;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Wav.Converters;

public interface IWavDecoder
{
    Sound? Decode(Stream stream);
}