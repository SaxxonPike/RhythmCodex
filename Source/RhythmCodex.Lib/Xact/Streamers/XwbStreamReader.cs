using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers;

[Service]
public class XwbStreamReader(
    IXwbHeaderStreamReader xwbHeaderStreamReader,
    IXwbDataStreamReader xwbDataStreamReader,
    IXwbEntryStreamReader xwbEntryStreamReader)
    : IXwbStreamReader
{
    public IEnumerable<XwbSound> Read(Stream source)
    {
        var reader = new BinaryReader(source);
        var sampleCount = 0;
        var entries = Array.Empty<XwbEntry>();
        var names = Array.Empty<string>();
        var dataChunk = Array.Empty<byte>();

        var header = xwbHeaderStreamReader.Read(source);

        for (var i = 0; i < (int) XwbSegIdx.Count; i++)
        {
            var region = header.Segments[i];
            if (region.Length <= 0)
                continue;

            source.Position = region.Offset;
            var buffer = reader.ReadBytes(region.Length);
            using var mem = new ReadOnlyMemoryStream(buffer);
            var memReader = new BinaryReader(mem);
            switch ((XwbSegIdx)i)
            {
                case XwbSegIdx.BankData:
                    var bank = xwbDataStreamReader.Read(mem);
                    sampleCount = bank.EntryCount;
                    entries = new XwbEntry[sampleCount];
                    names = new string[sampleCount];
                    break;
                case XwbSegIdx.EntryMetaData:
                    for (var j = 0; j < sampleCount; j++)
                        entries[j] = xwbEntryStreamReader.Read(mem);
                    break;
                case XwbSegIdx.EntryNames:
                    for (var j = 0; j < sampleCount; j++)
                        names[j] = memReader.ReadBytes(XwbConstants.WavebankEntrynameLength)
                            .TakeWhile(c => c != 0).ToArray().GetString();
                    break;
                case XwbSegIdx.EntryWaveData:
                    dataChunk = buffer;
                    break;
                case XwbSegIdx.SeekTables:
                    break;
                case XwbSegIdx.Count:
                default:
                    break;
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