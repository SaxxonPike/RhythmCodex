using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Dsl;

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

        protected byte[] GetEmbeddedResource(string name)
        {
            var assembly = GetType().Assembly;

            using (var stream = assembly.GetManifestResourceStream(name))
            using (var mem = new MemoryStream())
            {
                if (stream == null)
                    throw new IOException($"Embedded resource {name} was not found.");
                
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

        protected ICustomizationComposer<T> Build<T>()
        {
            return _fixture.Value.Build<T>();
        }

        protected T Create<T>()
        {
            return _fixture.Value.Create<T>();
        }

        protected T[] CreateMany<T>()
        {
            return _fixture.Value.CreateMany<T>().ToArray();
        }

        protected T[] CreateMany<T>(int count)
        {
            return _fixture.Value.CreateMany<T>(count).ToArray();
        }
    }
}
