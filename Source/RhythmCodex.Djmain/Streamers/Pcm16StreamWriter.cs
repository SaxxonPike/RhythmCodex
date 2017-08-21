using System.Collections.Generic;
using System.IO;
using RhythmCodex.Extensions;

namespace RhythmCodex.Djmain.Streamers
{
    public class Pcm16StreamWriter : IPcm16StreamWriter
    {
        private readonly IDjmainConfiguration _djmainConfiguration;

        public Pcm16StreamWriter(IDjmainConfiguration djmainConfiguration)
        {
            _djmainConfiguration = djmainConfiguration;
        }

        public void Write(Stream stream, IEnumerable<byte> data)
        {
            var writer = new BinaryWriter(stream);
            var arrayData = data.AsArray();
            var length = arrayData.Length & ~1;
            stream.Write(arrayData, 0, length);
            writer.Write(_djmainConfiguration.Pcm16EndMarker);
            writer.Write(_djmainConfiguration.Pcm16EndMarker);
        }
    }
}
