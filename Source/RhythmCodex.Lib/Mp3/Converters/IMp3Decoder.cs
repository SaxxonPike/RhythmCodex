using System.IO;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Mp3.Converters;

public interface IMp3Decoder
{
    ISound Decode(Stream stream);
}