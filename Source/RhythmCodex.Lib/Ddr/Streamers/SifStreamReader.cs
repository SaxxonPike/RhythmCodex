using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ddr.Streamers
{
    [Service]
    public class SifStreamReader : ISifStreamReader
    {
        public SifInfo Read(Stream stream, long length)
        {
            var data = new BinaryReader(stream).ReadBytes((int) length);
            return data[0] == 0x00 
                ? ReadBinarySif(data) 
                : ReadTextSif(data);
        }

        private SifInfo ReadTextSif(byte[] data)
        {
            var result = new SifInfo
            {
                KeyValues = new Dictionary<string, string>()
            };

            using (var dataStream = new MemoryStream(data))
            {
                foreach (var line in dataStream.ReadAllLines())
                {
                    if (!line.Contains("="))
                        continue;

                    var splits = line.Split('=');
                    var key = splits[0].Trim();
                    var val = string.Join(string.Empty, splits.Skip(1)).Trim();
                    result.KeyValues[key] = val;
                }
            }

            return result;
        }

        private SifInfo ReadBinarySif(byte[] data)
        {
            var result = new SifInfo
            {
                KeyValues = new Dictionary<string, string>()
            };

            result.KeyValues["version"] = "0";

            var text = new string[5];
            var chunkMem = new ReadOnlyMemoryStream(data);
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