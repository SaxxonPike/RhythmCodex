using System.IO;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Wav.Converters
{
    public interface IWavDecoder
    {
        ISound Decode(Stream stream);
    }
}