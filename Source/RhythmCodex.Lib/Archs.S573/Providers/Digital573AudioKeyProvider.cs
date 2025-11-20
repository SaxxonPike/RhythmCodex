using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using RhythmCodex.Archs.S573.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.S573.Providers;

[Service]
public class Digital573AudioKeyProvider : IDigital573AudioKeyProvider
{
    private readonly Lazy<Dictionary<string, Digital573AudioKey>> _keys = new(() =>
    {
        var archive = EmbeddedResources.GetArchive($"{typeof(Digital573AudioKeyProvider).Namespace}.Digital573AudioKeyDatabase.zip");

        var settings = new DataContractJsonSerializerSettings
        {
            UseSimpleDictionaryFormat = true
        };

        var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, int[]>), settings);

        var obj = serializer.ReadObject(new MemoryStream(archive.Single(e =>
            e.Key.Equals("db.json", StringComparison.OrdinalIgnoreCase)).Value));

        var dict = (Dictionary<string, int[]>)obj!;

        return dict
            .ToDictionary(kv => kv.Key, kv => new Digital573AudioKey
            {
                Values = kv.Value.ToList()
            });
    });

    public Digital573AudioKey? Get(ReadOnlySpan<byte> source)
    {
        var hash = string.Join(string.Empty, SHA1.HashData(source).Select(h => $"{h:X2}"));
        return _keys.Value.GetValueOrDefault(hash);
    }
}