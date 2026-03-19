using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;
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
        PsxMgsSoundTableBlock soundTable
    )
    {
        var bankEntries = psxMgsSoundBankDecoder
            .Decode(soundBank);

        var tableEntries = psxMgsSoundTableDecoder
            .Decode(soundTable);

        return tableEntries
            .Select(te =>
            {
                var sound = psxMgsSoundScriptRenderer.Render(te, bankEntries, 1);
                return sound;
            })
            .ToList();
    }
}