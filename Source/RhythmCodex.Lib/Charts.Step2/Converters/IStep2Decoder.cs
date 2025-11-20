using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Step2.Models;

namespace RhythmCodex.Charts.Step2.Converters;

public interface IStep2Decoder
{
    Chart Decode(Step2Chunk chunk);
}