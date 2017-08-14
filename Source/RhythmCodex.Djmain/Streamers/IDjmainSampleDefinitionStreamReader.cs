using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IDjmainSampleDefinitionStreamReader
    {
        IDictionary<int, DjmainSampleDefinition> Read(Stream stream);
    }
}