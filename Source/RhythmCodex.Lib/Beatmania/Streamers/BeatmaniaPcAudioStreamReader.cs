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
        private readonly IWavDecoder _wavDecoder;

        public BeatmaniaPcAudioStreamReader(IWavDecoder wavDecoder)
        {
            _wavDecoder = wavDecoder;
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
                yield return ReadInternal(source);
            }
        }

        private BeatmaniaPcAudioEntry ReadInternal(Stream source)
        {
            var reader = new BinaryReader(source);
            if (new string(reader.ReadChars(4)) != "2DX9")
                return null;

            var infoLength = reader.ReadInt32();
            var dataLength = reader.ReadInt32();
            var reserved = reader.ReadInt16();
            int channel = reader.ReadInt16();
            int panning = reader.ReadInt16();
            int volume = reader.ReadInt16();
            var options = reader.ReadInt32();
            var extraInfo = reader.ReadBytes(infoLength - 24);

            var wavData = reader.ReadBytes(dataLength);
            
            return new BeatmaniaPcAudioEntry
            {
                Channel = channel,
                Data = wavData,
                ExtraInfo = extraInfo,
                Options = options,
                Panning = panning,
                Reserved = reserved,
                Volume = volume
            };
        }
    }
}