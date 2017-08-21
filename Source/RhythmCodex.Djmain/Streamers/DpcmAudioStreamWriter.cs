using System.Collections.Generic;
using System.IO;
using RhythmCodex.Extensions;

namespace RhythmCodex.Djmain.Streamers
{
    public class DpcmAudioStreamWriter : IDpcmAudioStreamWriter
    {
        private readonly IDjmainConfiguration _djmainConfiguration;

        public DpcmAudioStreamWriter(IDjmainConfiguration djmainConfiguration)
        {
            _djmainConfiguration = djmainConfiguration;
        }

        public void Write(Stream stream, IEnumerable<byte> data)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(data.AsArray());
            writer.Write(_djmainConfiguration.DpcmEndMarker);
        }
    }
}
