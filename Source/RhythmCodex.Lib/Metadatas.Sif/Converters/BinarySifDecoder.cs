using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Sif.Models;

namespace RhythmCodex.Metadatas.Sif.Converters;

[Service]
public class BinarySifDecoder : IBinarySifDecoder
{
    public SifInfo Decode(ReadOnlyMemory<byte> bytes)
    {
        var result = new SifInfo
        {
            KeyValues = new Dictionary<string, string>()
        };

        result.KeyValues[SifKeys.Version] = "0";

        var text = new string[5];
        var chunkMem = new ReadOnlyMemoryStream(bytes);
        var reader = new BinaryReader(chunkMem, Encodings.Cp1252);
        var builder = new StringBuilder();
        chunkMem.Position = 0x001;
            
        for (var i = 0; i < text.Length; i++)
        {
            while (true)
            {
                var c = reader.ReadChar();
                if (c == 0)
                    break;
                builder.Append(c);
            }

            text[i] = builder.ToString();
            builder.Clear();
        }

        result.KeyValues[SifKeys.Dir] = text[0];
        result.KeyValues[SifKeys.Title] = text[1];
        result.KeyValues[SifKeys.Mix] = text[2];
        result.KeyValues[SifKeys.Artist] = text[3];
        result.KeyValues[SifKeys.Extra] = text[4];

        chunkMem.Position = 0x200;
        result.KeyValues[SifKeys.GrooveChartSingleLight] =
            $"{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()}";
        result.KeyValues[SifKeys.GrooveChartSingleStandard] =
            $"{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()}";
        result.KeyValues[SifKeys.GrooveChartSingleHeavy] =
            $"{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()}";
        result.KeyValues[SifKeys.GrooveChartDoubleLight] =
            $"{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()}";
        result.KeyValues[SifKeys.GrooveChartDoubleStandard] =
            $"{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()}";
        result.KeyValues[SifKeys.GrooveChartDoubleHeavy] =
            $"{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()}";
        result.KeyValues[SifKeys.FootSingle] =
            $"{reader.ReadByte()},{reader.ReadByte()},{reader.ReadByte()}";
        result.KeyValues[SifKeys.FootDouble] =
            $"{reader.ReadByte()},{reader.ReadByte()},{reader.ReadByte()}";
        result.KeyValues[SifKeys.BpmMin] = 
            $"{reader.ReadInt16()}";
        result.KeyValues[SifKeys.BpmMax] = 
            $"{reader.ReadInt16()}";
        result.KeyValues[SifKeys.EndBar] =
            $"{reader.ReadInt16()}";
            
        return result;
    }
}