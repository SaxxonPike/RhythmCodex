using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Vag.Streamers;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Archs.Psx.Converters;

/// <inheritdoc />
[Service]
public sealed class PsxMgsSoundBankDecoder(IVagStreamReader vagStreamReader)
    : IPsxMgsSoundBankDecoder
{
    /// <inheritdoc />
    public List<PsxMgsSoundBankEntryWithData> Decode(PsxMgsSoundBankBlock block)
    {
        //
        // The keysound block consists of patches that are applied to
        // an address space.
        //

        var ram = new byte[0x80000];

        foreach (var patch in block.Patches)
            patch.Data.Span.CopyTo(ram.AsSpan(patch.Address));

        //
        // Once applied, parse the keysound table. Only indices
        // 128-255 are valid for keysound samples.
        //

        var infos = new Dictionary<int, PsxMgsSoundBankEntry>();

        for (var i = 128; i < 256; i++)
        {
            var bytes = ram.AsSpan(i << 4, 16);

            var info = new PsxMgsSoundBankEntry
            {
                Offset = bytes.AsS32L(),
                Note = bytes[0x4],
                Tune = bytes[0x5],
                AttackMode = bytes[0x6],
                AttackRate = bytes[0x7],
                DecayRate = bytes[0x8],
                SustainMode = bytes[0x9],
                SustainRate = bytes[0xA],
                SustainLevel = bytes[0xB],
                ReleaseMode = bytes[0xC],
                ReleaseRate = bytes[0xD],
                Pan = bytes[0xE],
                DeclVol = bytes[0xF]
            };

            if (info.Offset < 0x1000 || info.Offset >= ram.Length)
                continue;

            infos.Add(i, info);
        }

        //
        // For each keysound we have established is valid, extract the
        // audio chunk.
        //

        var result = new List<PsxMgsSoundBankEntryWithData>();
        using var ramStream = new MemoryStream(ram);

        foreach (var (id, info) in infos)
        {
            ramStream.Position = info.Offset;
            var audio = vagStreamReader.Read(ramStream, 1, 16);

            if (audio != null)
            {
                result.Add(new PsxMgsSoundBankEntryWithData
                {
                    Index = id,
                    Entry = info,
                    Data = audio.Data
                });
            }
        }

        return result;
    }
}