using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

[Service]
public class XsbHeaderStreamReader : IXsbHeaderStreamReader
{
    public XsbHeader Read(Stream stream)
    {
        var reader = new BinaryReader(stream);

        var result = new XsbHeader
        {
            Signature = reader.ReadInt32(),
            ToolVersion = reader.ReadInt16(),
            FormatVersion = reader.ReadInt16(),
            Crc = reader.ReadInt16(),
            BuildTime = reader.ReadInt64(),
            Platform = reader.ReadByte(),
            SimpleCueCount = reader.ReadInt16(),
            ComplexCueCount = reader.ReadInt16(),
            Unk0 = reader.ReadInt16(),
            TotalCueCount = reader.ReadInt16(),
            WaveBankCount = reader.ReadByte(),
            SoundCount = reader.ReadInt16(),
            CueNameTableLength = reader.ReadInt16(),
            Unk1 = reader.ReadInt16(),
            SimpleCuesOffset = reader.ReadInt32(),
            ComplexCuesOffset = reader.ReadInt32(),
            CueNamesOffset = reader.ReadInt32(),
            Unk2 = reader.ReadInt32(),
            VariationTablesOffset = reader.ReadInt32(),
            Unk3 = reader.ReadInt32(),
            WaveBankNameTableOffset = reader.ReadInt32(),
            CueNameHashTableOffset = reader.ReadInt32(),
            CueNameHashValuesOffset = reader.ReadInt32(),
            SoundsOffset = reader.ReadInt32(),
            Name = reader.ReadBytes(XwbConstants.WavebankBanknameLength)
                .TakeWhile(b => b != 0x00).ToArray().GetString()
        };

        return result;
    }
}