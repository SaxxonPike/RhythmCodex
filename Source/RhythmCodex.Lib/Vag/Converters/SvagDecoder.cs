using System;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters;

[Service]
public class SvagDecoder(IVagDecoder vagDecoder) : ISvagDecoder
{
    public Sound Decode(SvagContainer container)
    {
        ArgumentNullException.ThrowIfNull(container, nameof(container));
        
        var decoded = vagDecoder.Decode(container.VagChunk);
        decoded[NumericData.Rate] = container.SampleRate;
        return decoded;
    }
}