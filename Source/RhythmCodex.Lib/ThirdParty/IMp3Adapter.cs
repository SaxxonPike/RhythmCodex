using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.ThirdParty
{
    public interface IMp3Adapter
    {
        ISound Decode(Stream stream);
    }
}