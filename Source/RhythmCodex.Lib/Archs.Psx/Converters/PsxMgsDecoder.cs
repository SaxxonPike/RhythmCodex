using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Psx.Converters;

[Service]
public sealed class PsxMgsDecoder(
    IPsxMgsSoundTableDecoder psxMgsSoundTableDecoder,
    IPsxMgsSoundBankDecoder psxMgsSoundBankDecoder,
    IPsxMgsSoundScriptRenderer psxMgsSoundScriptRenderer)
    : IPsxMgsDecoder
{
    public List<Sound> DecodeSounds(
        PsxMgsSoundBankBlock soundBank,
        PsxMgsSoundTableBlock soundTable,
        int sampleRate
    )
    {
        var bankEntries = psxMgsSoundBankDecoder
            .Decode(soundBank);

        var tableEntries = psxMgsSoundTableDecoder
            .Decode(soundTable);

        return tableEntries
            .Where(te => te.Channels.Count > 0)
            .Select(te =>
            {
                var sound = psxMgsSoundScriptRenderer.Render(te, bankEntries, 1, sampleRate);
                sound[NumericData.Id] = te.Index;
                return sound;
            })
            .ToList();
    }
}