using System.Collections.Generic;

namespace RhythmCodex.Djmain.Model
{
    public interface IDjmainSample
    {
        IList<byte> Data { get; }
        IDjmainSampleInfo Info { get; }
    }
}