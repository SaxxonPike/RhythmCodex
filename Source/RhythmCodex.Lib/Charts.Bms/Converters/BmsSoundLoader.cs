using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Streamers;

namespace RhythmCodex.Charts.Bms.Converters;

[Service]
public class BmsSoundLoader(
    ISoundStreamReader soundStreamReader
) : IBmsSoundLoader
{
    private readonly Dictionary<string, Func<Stream, Sound?>> _extensions = new()
    {
        { "wav", soundStreamReader.Read },
        { "flac", soundStreamReader.Read },
        { "ogg", soundStreamReader.Read },
        { "mp3", soundStreamReader.Read }
    };

    public List<Sound> Load(IDictionary<int, string> map, IFileAccessor accessor)
    {
        return LoadInternal(map, accessor).ToList();
    }

    private IEnumerable<Sound> LoadInternal(IDictionary<int, string> map, IFileAccessor accessor)
    {
        foreach (var kv in map)
        {
            var decoder = GetDecoder(Path.GetFileNameWithoutExtension(kv.Value), accessor);
            if (decoder.Decoder == null)
                continue;

            using var stream = accessor.OpenRead(decoder.Filename);
            Sound? decoded;

            try
            {
                decoded = decoder.Decoder(stream)!;
                decoded[NumericData.Id] = kv.Key;
            }
            catch
            {
                decoded = null;
            }

            if (decoded != null)
                yield return decoded;
        }
    }

    private (string Filename, Func<Stream, Sound?>? Decoder) GetDecoder(string name, IFileAccessor accessor)
    {
        var file = accessor.GetFileNameByExtension(name, _extensions.Keys);
        
        return file == null 
            ? (name, null) 
            : (file.Filename, _extensions[file.Extension]);
    }
}