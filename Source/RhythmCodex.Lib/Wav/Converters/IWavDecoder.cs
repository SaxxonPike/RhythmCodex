using System.IO;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Wav.Converters
{
    public interface IWavDecoder
    {
        ISound Decode(Stream stream);
    }
}