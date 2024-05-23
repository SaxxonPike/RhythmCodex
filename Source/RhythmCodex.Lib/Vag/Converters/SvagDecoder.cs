using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters;

[Service]
public class SvagDecoder : ISvagDecoder
{
    private readonly IVagDecoder _vagDecoder;

    public SvagDecoder(IVagDecoder vagDecoder)
    {
        _vagDecoder = vagDecoder;
    }
        
    public ISound Decode(SvagContainer container)
    {
        var decoded = _vagDecoder.Decode(container.VagChunk);
        decoded[NumericData.Rate] = container.SampleRate;
        return decoded;
    }
}