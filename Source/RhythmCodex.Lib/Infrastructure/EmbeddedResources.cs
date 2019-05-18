using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace RhythmCodex.Infrastructure
{
    internal static class EmbeddedResources
    {
        public static byte[] Get(string name, Assembly assembly = null)
        {
            //var resources = typeof(EmbeddedResources).Assembly.GetManifestResourceNames();
            using (var stream =
                (assembly ?? typeof(EmbeddedResources).Assembly).GetManifestResourceStream(name))
            using (var mem = new MemoryStream())
            {
                if (stream == null)
                    throw new IOException($"Embedded resource {name} was not found.");

                stream.CopyTo(mem);
                return mem.ToArray();
            }
        }
        
        public static IDictionary<string, byte[]> GetArchive(string name)
        {
            var output = new Dictionary<string, byte[]>();

            using (var mem = new MemoryStream(Get(name)))
            using (var archive = new ZipArchive(mem, ZipArchiveMode.Read, false))
            {
                foreach (var entry in archive.Entries)
                    using (var entryStream = entry.Open())
                    using (var entryCopy = new MemoryStream())
                    {
                        entryStream.CopyTo(entryCopy);
                        entryCopy.Flush();
                        output[entry.FullName] = entryCopy.ToArray();
                    }
            }

            return output;
        }
    }
}