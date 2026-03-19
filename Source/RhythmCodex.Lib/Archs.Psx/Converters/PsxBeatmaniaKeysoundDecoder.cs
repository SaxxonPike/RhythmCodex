using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Converters;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Archs.Psx.Converters;

/// <inheritdoc />
[Service]
public class PsxBeatmaniaKeysoundDecoder(IVagDecoder vagDecoder) : IPsxBeatmaniaKeysoundDecoder
{
    /// <inheritdoc />
    public Sound Decode(PsxBeatmaniaKeysound keysound)
    {
        var sound = vagDecoder.Decode(new VagChunk
        {
            Data = keysound.Data,
            Channels = 1
        });

        sound[NumericData.Id] = keysound.Index;
        sound[NumericData.Rate] = 37800;
        return sound;
    }
}