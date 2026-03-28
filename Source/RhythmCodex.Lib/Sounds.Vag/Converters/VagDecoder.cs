using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Sounds.Vag.Converters;

[Service]
public sealed class VagDecoder(IVagSplitter vagSplitter) : IVagDecoder
{
    public Sound Decode(VagChunk chunk)
    {
        return new Sound
        {
            Samples = vagSplitter.Split(chunk)
        };
    }
}