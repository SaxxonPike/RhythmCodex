using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Providers
{
    [Service]
    public class Ddr573AudioKeyProvider : IDdr573AudioKeyProvider
    {
        private readonly Lazy<Dictionary<string, int[]>> _keys = new Lazy<Dictionary<string, int[]>>(() =>
        {
            var archive = EmbeddedResources.GetArchive("RhythmCodex.Ddr.Providers.Ddr573AudioKeyDatabase.zip");
            var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, int[]>), new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true
            });
            var obj = serializer.ReadObject(new MemoryStream(archive.Single(e =>
                e.Key.Equals("db.json", StringComparison.OrdinalIgnoreCase)).Value));
            return (Dictionary<string, int[]>) obj;
        });

        public int[] Get(byte[] source)
        {
            using (var sha = SHA1.Create())
            {
                var hash = string.Join(string.Empty, sha.ComputeHash(source).Select(h => $"{h:X2}"));
                return _keys.Value.ContainsKey(hash)
                    ? _keys.Value[hash]
                    : null;
            }
        }
    }
}