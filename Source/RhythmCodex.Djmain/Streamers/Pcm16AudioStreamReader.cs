using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RhythmCodex.Djmain.Streamers
{
    public class Pcm16AudioStreamReader : IPcm16AudioStreamReader
    {
        private readonly IDjmainConfiguration _config;

        public Pcm16StreamReader(IDjmainConfiguration config)
        {
            _config = config;
        }

    }
}
