using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Sif.Models;

namespace RhythmCodex.Metadatas.Sif.Converters;

[Service]
public class TextSifDecoder : ITextSifDecoder
{
    public SifInfo Decode(IEnumerable<string> lines)
    {
        var result = new SifInfo
        {
            KeyValues = new Dictionary<string, string>()
        };

        foreach (var line in lines)
        {
            if (!line.Contains("="))
                continue;

            var splits = line.Split('=');
            var key = splits[0].Trim();
            var val = string.Join(string.Empty, splits.Skip(1)).Trim();
            result.KeyValues[key] = val;
        }

        return result;
    }
}