using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Mgs.Models;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Vag.Streamers;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Mgs.Converters;

/// <inheritdoc />
[Service]
public sealed class MgsSdSoundBankDecoder(IVagStreamReader vagStreamReader)
    : IMgsSdSoundBankDecoder
{
    /// <inheritdoc />
    public List<MgsSdSoundBankEntryWithData> Decode(MgsSdSoundBankBlock block)
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

        var infos = new Dictionary<int, MgsSdSoundBankEntry>();

        for (var i = 128; i < 256; i++)
        {
            var bytes = ram.AsSpan(i << 4, 16);

            var info = new MgsSdSoundBankEntry
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

        var result = new List<MgsSdSoundBankEntryWithData>();
        using var ramStream = new MemoryStream(ram);

        foreach (var (id, info) in infos)
        {
            ramStream.Position = info.Offset;
            var audio = vagStreamReader.Read(ramStream, 1, 16);

            if (audio != null)
            {
                result.Add(new MgsSdSoundBankEntryWithData
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