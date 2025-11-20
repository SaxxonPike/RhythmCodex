using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Djmain.Streamers;

[Service]
public class DjmainSampleInfoStreamReader : IDjmainSampleInfoStreamReader
{
    public Dictionary<int, DjmainSampleInfo> Read(Stream stream, int maxSize)
    {
        return ReadInternal(stream, maxSize).ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    private IEnumerable<KeyValuePair<int, DjmainSampleInfo>> ReadInternal(Stream stream, int maxSize)
    {
        var resultList = new Dictionary<int, DjmainSampleInfo>();
        var maxDefs = maxSize / 11;
        Span<byte> buffer = stackalloc byte[11];

        for (var i = 0; i < maxDefs; i++)
        {
            var bytesRead = stream.Read(buffer);
            if (bytesRead < 11)
                break;

            var result = new DjmainSampleInfo
            {
                Channel = buffer[0],
                Frequency = BinaryPrimitives.ReadUInt16LittleEndian(buffer[1..]),
                ReverbVolume = buffer[3],
                Volume = buffer[4],
                Panning = buffer[5],
                Offset = BinaryPrimitives.ReadUInt16LittleEndian(buffer[6..]) | ((uint) buffer[8] << 16),
                SampleType = buffer[9],
                Flags = buffer[10]
            };

            resultList.Add(i, result);
        }

        return resultList;
    }
}