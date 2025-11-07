using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Pc.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Pc.Streamers;

[Service]
public class BeatmaniaPcAudioStreamReader(IBeatmaniaPcAudioEntryStreamReader beatmaniaPcAudioEntryStreamReader)
    : IBeatmaniaPcAudioStreamReader
{
    public IEnumerable<BeatmaniaPcAudioEntry> Read(Stream source, long length)
    {
        var baseOffset = source.Position;
        var reader = new BinaryReader(source);

        reader.ReadBytes(0x10);

        var headerLength = reader.ReadInt32();
        var sampleCount = reader.ReadInt32();
        var sampleOffset = new long[sampleCount];

        reader.ReadBytes(0x30);

        for (var i = 0; i < sampleCount; i++)
            sampleOffset[i] = reader.ReadInt32();

        for (var i = 0; i < sampleCount; i++)
        {
            reader.BaseStream.Position = sampleOffset[i] + baseOffset;
            yield return beatmaniaPcAudioEntryStreamReader.Read(source);
        }
    }
}