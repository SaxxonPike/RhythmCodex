using System.IO;
using System.Text;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[Service]
public class SsqInfoChunkDecoder : ISsqInfoChunkDecoder
{
    public SsqInfoChunk Decode(SsqChunk ssqChunk)
    {
        var text = new string[12];
        var chunkMem = new ReadOnlyMemoryStream(ssqChunk.Data);
        var reader = new BinaryReader(chunkMem, Encodings.CP1252);
        var builder = new StringBuilder();
            
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
            
        return new SsqInfoChunk
        {
            Text = text,
            Difficulties = reader.ReadBytes(10)
        };
    }
}