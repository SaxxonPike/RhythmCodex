using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Extensions;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;
using RhythmCodex.Wav.Models;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XwbStreamReader : IXwbStreamReader
    {
        private readonly IXwbDataStreamReader _xwbDataStreamReader;
        private readonly IXwbEntryStreamReader _xwbEntryStreamReader;
        private readonly IXwbHeaderStreamReader _xwbHeaderStreamReader;

        public XwbStreamReader(
            IXwbHeaderStreamReader xwbHeaderStreamReader,
            IXwbDataStreamReader xwbDataStreamReader,
            IXwbEntryStreamReader xwbEntryStreamReader)
        {
            _xwbDataStreamReader = xwbDataStreamReader;
            _xwbEntryStreamReader = xwbEntryStreamReader;
            _xwbHeaderStreamReader = xwbHeaderStreamReader;
        }

        public IEnumerable<XwbSound> Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var sampleCount = 0;
            XwbEntry[] entries = { };
            string[] names = { };
            var dataChunk = new byte[0];

            var header = _xwbHeaderStreamReader.Read(source);

            for (var i = 0; i < (int) XwbSegIdx.Count; i++)
            {
                var region = header.Segments[i];
                if (region.Length <= 0)
                    continue;

                source.Position = region.Offset;
                var buffer = reader.ReadBytes(region.Length);
                using (var mem = new MemoryStream(buffer))
                {
                    var memReader = new BinaryReader(mem);
                    switch (i)
                    {
                        case (int) XwbSegIdx.BankData:
                            var bank = _xwbDataStreamReader.Read(mem);
                            sampleCount = bank.EntryCount;
                            entries = new XwbEntry[sampleCount];
                            names = new string[sampleCount];
                            break;
                        case (int) XwbSegIdx.EntryMetaData:
                            for (var j = 0; j < sampleCount; j++)
                                entries[j] = _xwbEntryStreamReader.Read(mem);
                            break;
                        case (int) XwbSegIdx.EntryNames:
                            for (var j = 0; j < sampleCount; j++)
                                names[j] = new string(memReader.ReadChars(XwbConstants.WavebankEntrynameLength)
                                    .TakeWhile(c => c != 0).ToArray());
                            break;
                        case (int) XwbSegIdx.EntryWaveData:
                            dataChunk = buffer;
                            break;
                        case (int) XwbSegIdx.SeekTables:
                            break;
                        default:
                            break;
                    }                    
                }
            }

            return Enumerable
                .Range(0, sampleCount)
                .Select(i =>
                {
                    var entry = entries[i];
                    var data = dataChunk.AsSpan(entry.PlayRegion.Offset, entry.PlayRegion.Length).ToArray();
                    return new XwbSound
                    {
                        Data = data,
                        Info = entry,
                        Name = names[i]
                    };
                });
        }
    }
}