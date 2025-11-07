using System.Collections.Generic;
using RhythmCodex.Archs.Djmain.Model;

namespace RhythmCodex.Archs.Djmain.Heuristics;

public interface IDjmainOffsetProvider
{
    List<int> GetChartOffsets(DjmainChunkFormat format);
    int GetSoundOffset(DjmainChunkFormat format);
    List<int> GetSampleMapOffsets(DjmainChunkFormat format);
    List<string> GetChartNames(DjmainChunkFormat format);
    List<int> GetSampleChartMap(DjmainChunkFormat format);
}