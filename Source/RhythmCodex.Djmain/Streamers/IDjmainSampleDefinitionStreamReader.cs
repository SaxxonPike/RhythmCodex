using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IDjmainSampleDefinitionStreamReader
    {
        IEnumerable<DjmainSampleDefinition> Read(Stream stream);
    }
}