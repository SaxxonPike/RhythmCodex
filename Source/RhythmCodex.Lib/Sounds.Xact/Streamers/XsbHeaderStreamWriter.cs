using System;
using System.IO;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

[Service]
public class XsbHeaderStreamWriter : IXsbHeaderStreamWriter
{
    public int Write(Stream stream, XsbHeader header)
    {
        var start = stream.Position;
        var writer = new BinaryWriter(stream);

        writer.Write(header.Signature);
        writer.Write(header.ToolVersion);
        writer.Write(header.FormatVersion);
        writer.Write(header.Crc);
        writer.Write(header.BuildTime);
        writer.Write(header.Platform);
        writer.Write(header.SimpleCueCount);
        writer.Write(header.ComplexCueCount);
        writer.Write(header.Unk0);
        writer.Write(header.TotalCueCount);
        writer.Write(header.WaveBankCount);
        writer.Write(header.SoundCount);
        writer.Write(header.CueNameTableLength);
        writer.Write(header.Unk1);
        writer.Write(header.SimpleCuesOffset);
        writer.Write(header.ComplexCuesOffset);
        writer.Write(header.CueNamesOffset);
        writer.Write(header.Unk2);
        writer.Write(header.VariationTablesOffset);
        writer.Write(header.Unk3);
        writer.Write(header.WaveBankNameTableOffset);
        writer.Write(header.CueNameHashTableOffset);
        writer.Write(header.CueNameHashValuesOffset);
        writer.Write(header.SoundsOffset);

        Span<byte> name = stackalloc byte[XwbConstants.WavebankBanknameLength];
        header.Name.GetBytes().AsSpan().CopyTo(name);
        writer.Write(name);

        return (int) (stream.Position - start);
    }
}