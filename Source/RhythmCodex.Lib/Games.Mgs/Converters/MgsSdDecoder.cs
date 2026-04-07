using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Games.Mgs.Models;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Games.Mgs.Converters;

[Service]
public sealed class MgsSdDecoder(
    IMgsSdSoundTableDecoder mgsSdSoundTableDecoder,
    IMgsSdSoundBankDecoder mgsSdSoundBankDecoder,
    IMgsSdSoundScriptRenderer mgsSdSoundScriptRenderer)
    : IMgsSdDecoder
{
    public List<MgsSdSoundDecodeResult> DecodeSounds(
        MgsSdSoundBankBlock soundBank,
        MgsSdSoundTableBlock soundTable,
        int sampleRate
    )
    {
        var bankEntries = mgsSdSoundBankDecoder
            .Decode(soundBank);

        var tableEntries = mgsSdSoundTableDecoder
            .Decode(soundTable);

        return tableEntries
            .Where(te => te.Channels.Count > 0)
            .Select(te =>
            {
                var sound = mgsSdSoundScriptRenderer.Render(te, bankEntries, sampleRate);
                sound[NumericData.Id] = te.Index;
                sound[NumericData.Volume] = 0.8f;
                return new MgsSdSoundDecodeResult
                {
                    Index = te.Index,
                    Packets = te.Channels,
                    Sound = sound
                };
            })
            .Where(s => s.Sound.Samples.Count > 0 && s.Sound.Samples.Any(x => x.Data.Length > 0))
            .ToList();
    }
}