using System.Collections.Generic;
using System.IO;
using RhythmCodex.Extensions;

namespace RhythmCodex.Djmain.Streamers
{
    public class Pcm8StreamWriter : IPcm8StreamWriter
    {
        private readonly IDjmainConfiguration _djmainConfiguration;

        public Pcm8StreamWriter(IDjmainConfiguration djmainConfiguration)
        {
            _djmainConfiguration = djmainConfiguration;
        }

        public void Write(Stream stream, IEnumerable<byte> data)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(data.AsArray());
            writer.Write(_djmainConfiguration.Pcm8EndMarker);
        }
    }
}
