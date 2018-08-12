using System.Collections.Generic;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Heuristics
{
    public interface IDjmainOffsetProvider
    {
        ICollection<int> GetChartOffsets(DjmainChunkFormat format);
        int GetSoundOffset(DjmainChunkFormat format);
        ICollection<int> GetSampleMapOffsets(DjmainChunkFormat format);
        ICollection<string> GetChartNames(DjmainChunkFormat format);
    }
}