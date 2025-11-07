using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Flac.Converters;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Mp3.Converters;
using RhythmCodex.Sounds.Ogg.Converters;
using RhythmCodex.Sounds.Wav.Converters;

namespace RhythmCodex.Charts.Bms.Converters;

[Service]
public class BmsSoundLoader(
    IWavDecoder wavDecoder,
    IMp3Decoder mp3Decoder,
    IOggDecoder oggDecoder,
    IFlacDecoder flacDecoder)
    : IBmsSoundLoader
{
    private readonly Dictionary<string, Func<Stream, Sound?>> _extensions = new()
    {
        { "wav", wavDecoder.Decode },
        { "flac", flacDecoder.Decode },
        { "ogg", oggDecoder.Decode },
        { "mp3", mp3Decoder.Decode }
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