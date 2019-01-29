using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sif.Models;

namespace RhythmCodex.Sif.Converters
{
    [Service]
    public class BinarySifDecoder : IBinarySifDecoder
    {
        public SifInfo Decode(ReadOnlyMemory<byte> bytes)
        {
            var result = new SifInfo
            {
                KeyValues = new Dictionary<string, string>()
            };

            result.KeyValues["version"] = "0";

            var text = new string[5];
            var chunkMem = new ReadOnlyMemoryStream(bytes);
            var reader = new BinaryReader(chunkMem, Encodings.CP1252);
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

            result.KeyValues["dir"] = text[0];
            result.KeyValues["title"] = text[1];
            result.KeyValues["mix"] = text[2];
            result.KeyValues["artist"] = text[3];
            result.KeyValues["extra"] = text[4];

            chunkMem.Position = 0x200;
            result.KeyValues["groove_chart.single.light"] =
                $"{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()}";
            result.KeyValues["groove_chart.single.standard"] =
                $"{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()}";
            result.KeyValues["groove_chart.single.heavy"] =
                $"{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()}";
            result.KeyValues["groove_chart.double.light"] =
                $"{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()}";
            result.KeyValues["groove_chart.double.standard"] =
                $"{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()}";
            result.KeyValues["groove_chart.double.heavy"] =
                $"{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()},{reader.ReadInt16()}";
            result.KeyValues["foot.single"] =
                $"{reader.ReadByte()},{reader.ReadByte()},{reader.ReadByte()}";
            result.KeyValues["foot.double"] =
                $"{reader.ReadByte()},{reader.ReadByte()},{reader.ReadByte()}";
            result.KeyValues["bpm_min"] = 
                $"{reader.ReadInt16()}";
            result.KeyValues["bpm_max"] = 
                $"{reader.ReadInt16()}";
            result.KeyValues["end_bar"] =
                $"{reader.ReadInt16()}";
            
            return result;
        }
    }
}