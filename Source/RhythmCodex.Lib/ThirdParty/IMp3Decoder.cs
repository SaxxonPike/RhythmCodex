using System.IO;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.ThirdParty;

public interface IMp3Decoder
{
    ISound Decode(Stream stream);
}