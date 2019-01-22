using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Wav.Converters;

namespace RhythmCodex.Beatmania.Streamers
{
    [Service]
    public class BeatmaniaPcAudioStreamReader : IBeatmaniaPcAudioStreamReader
    {
        private readonly IBeatmaniaPcAudioEntryStreamReader _beatmaniaPcAudioEntryStreamReader;

        public BeatmaniaPcAudioStreamReader(
            IBeatmaniaPcAudioEntryStreamReader beatmaniaPcAudioEntryStreamReader)
        {
            _beatmaniaPcAudioEntryStreamReader = beatmaniaPcAudioEntryStreamReader;
        }

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
                yield return _beatmaniaPcAudioEntryStreamReader.Read(source);
            }
        }
    }
}