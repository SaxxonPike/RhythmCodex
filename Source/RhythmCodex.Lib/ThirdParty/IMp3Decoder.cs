using System.IO;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.ThirdParty
{
    public interface IMp3Decoder
    {
        ISound Decode(Stream stream);
    }
}