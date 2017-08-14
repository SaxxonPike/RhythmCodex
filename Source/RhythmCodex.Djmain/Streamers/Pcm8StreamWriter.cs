using System.Collections.Generic;
using System.IO;
using RhythmCodex.Extensions;

namespace RhythmCodex.Djmain.Streamers
{
    public class Pcm8StreamWriter : IPcm8StreamWriter
    {
        public void Write(Stream stream, IEnumerable<byte> data)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(data.AsArray());
            writer.Write(DjmainConstants.Pcm8EndMarker);
        }
    }
}
