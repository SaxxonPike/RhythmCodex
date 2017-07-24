using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Ploeh.AutoFixture;

namespace RhythmCodex
{
    public class BaseTestFixture
    {
        private readonly Lazy<Fixture> _fixture = new Lazy<Fixture>(() =>
        {
            var fixture = new Fixture();
            new SupportMutableValueTypesCustomization().Customize(fixture);
            return fixture;
        });

        protected Fixture Fixture => _fixture.Value;
        
        protected byte[] GetEmbeddedResource(string name)
        {
            var assembly = GetType().Assembly;

            using (var stream = assembly.GetManifestResourceStream(name))
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                return mem.ToArray();
            }
        }

        protected IDictionary<string, byte[]> GetArchiveResource(string name)
        {
            var output = new Dictionary<string, byte[]>();
            
            using (var mem = new MemoryStream(GetEmbeddedResource(name)))
            using (var archive = new ZipArchive(mem, ZipArchiveMode.Read, false))
            {
                foreach (var entry in archive.Entries)
                {
                    using (var entryStream = entry.Open())
                    using (var entryCopy = new MemoryStream())
                    {
                        entryStream.CopyTo(entryCopy);
                        entryCopy.Flush();
                        output[entry.FullName] = entryCopy.ToArray();
                    }
                }
            }

            return output;
        }
    }
}
