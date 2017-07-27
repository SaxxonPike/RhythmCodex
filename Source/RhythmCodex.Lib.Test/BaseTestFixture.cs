using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace RhythmCodex
{
    public class BaseTestFixture
    {
        [SetUp]
        public void __Setup()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        [TearDown]
        public void __Teardown()
        {
            _stopwatch.Stop();
            TestContext.Out.WriteLine($"{TestContext.CurrentContext.Test.FullName}: {_stopwatch.ElapsedMilliseconds}ms");
        }

        private Stopwatch _stopwatch;
        
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
