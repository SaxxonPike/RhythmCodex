using System.Collections.Generic;
using System.IO;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Streamers
{
    [Service]
    public class DjmainAudioStreamWriter : IDjmainAudioStreamWriter
    {
        public void WriteDpcm(Stream stream, IEnumerable<byte> data)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(data.AsArray());
            writer.Write(DjmainConstants.DpcmEndMarker);
        }

        public void WritePcm8(Stream stream, IEnumerable<byte> data)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(data.AsArray());
            writer.Write(DjmainConstants.Pcm8EndMarker);
        }

        public void WritePcm16(Stream stream, IEnumerable<byte> data)
        {
            var writer = new BinaryWriter(stream);
            var arrayData = data.AsArray();
            var length = arrayData.Length & ~1;
            stream.Write(arrayData, 0, length);
            writer.Write(DjmainConstants.Pcm16EndMarker);
            writer.Write(DjmainConstants.Pcm16EndMarker);
        }
    }
}