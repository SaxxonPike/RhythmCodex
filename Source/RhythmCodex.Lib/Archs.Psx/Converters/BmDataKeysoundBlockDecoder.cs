using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Vag.Streamers;

namespace RhythmCodex.Archs.Psx.Converters;

[Service]
public class BmDataKeysoundBlockDecoder(IVagStreamReader vagStreamReader)
    : IBmDataKeysoundBlockDecoder
{
    public List<BmDataKeysound> Decode(BmDataKeysoundBlock block)
    {
        //
        // The keysound block consists of patches that are applied to
        // an address space.
        //

        var ram = new byte[0x200000];

        foreach (var patch in block.Patches)
            patch.Data.Span.CopyTo(ram.AsSpan(patch.Address));

        //
        // Once applied, parse the keysound table.
        //

        const int count = 256;
        var infos = new Dictionary<int, BmDataKeysoundInfo>();

        for (var i = 0; i < count; i++)
        {
            var bytes = ram.AsSpan(i << 4, 16);

            var info = new BmDataKeysoundInfo
            {
                Offset = ReadInt32LittleEndian(bytes),
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

        var result = new List<BmDataKeysound>();
        using var ramStream = new MemoryStream(ram);

        foreach (var (id, info) in infos)
        {
            ramStream.Position = info.Offset;
            var audio = vagStreamReader.Read(ramStream, 1, 16);

            if (audio != null)
            {
                result.Add(new BmDataKeysound
                {
                    Index = id + 2,
                    Info = info,
                    Data = audio.Data
                });
            }
        }

        return result;
    }
}