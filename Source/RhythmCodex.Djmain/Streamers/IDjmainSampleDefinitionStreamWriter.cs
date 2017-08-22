using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IDjmainSampleDefinitionStreamWriter
    {
        void Write(Stream stream, IEnumerable<KeyValuePair<int, DjmainSampleInfo>> definitions);
    }
}