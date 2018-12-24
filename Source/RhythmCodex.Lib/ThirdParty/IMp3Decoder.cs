using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.ThirdParty
{
    public interface IMp3Decoder
    {
        ISound Decode(Stream stream);
    }
}