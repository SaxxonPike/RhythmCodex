using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Archs.Psx.Converters;

[Service]
public sealed class PsxMgsDecoder(
    IPsxMgsSoundTableDecoder psxMgsSoundTableDecoder,
    IPsxMgsSoundBankDecoder psxMgsSoundBankDecoder,
    IPsxMgsSoundScriptRenderer psxMgsSoundScriptRenderer)
    : IPsxMgsDecoder
{
    public List<PsxMgsSoundDecodeResult> DecodeSounds(
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
                var sound = psxMgsSoundScriptRenderer.Render(te, bankEntries, sampleRate);
                sound[NumericData.Id] = te.Index;
                sound[NumericData.Volume] = 0.8f;
                return new PsxMgsSoundDecodeResult
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