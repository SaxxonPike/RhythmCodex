using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Wav.Converters
{
    public interface IWavDecoder
    {
        ISound Decode(Stream stream);
    }
}