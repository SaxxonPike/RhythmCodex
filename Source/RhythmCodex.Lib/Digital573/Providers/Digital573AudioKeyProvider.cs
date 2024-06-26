using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Digital573.Providers;

[Service]
public class Digital573AudioKeyProvider : IDigital573AudioKeyProvider
{
    private readonly Lazy<Dictionary<string, int[]>> _keys = new(() =>
    {
        var archive = EmbeddedResources.GetArchive("RhythmCodex.Digital573.Providers.Digital573AudioKeyDatabase.zip");
        var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, int[]>), new DataContractJsonSerializerSettings
        {
            UseSimpleDictionaryFormat = true
        });
        var obj = serializer.ReadObject(new MemoryStream(archive.Single(e =>
            e.Key.Equals("db.json", StringComparison.OrdinalIgnoreCase)).Value));
        return (Dictionary<string, int[]>) obj!;
    });

    public int[]? Get(byte[] source)
    {
        var hash = string.Join(string.Empty, SHA1.HashData(source).Select(h => $"{h:X2}"));
        return _keys.Value.GetValueOrDefault(hash);
    }
}