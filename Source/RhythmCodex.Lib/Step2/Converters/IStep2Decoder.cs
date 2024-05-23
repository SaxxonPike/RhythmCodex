using RhythmCodex.Charting.Models;
using RhythmCodex.Step2.Models;

namespace RhythmCodex.Step2.Converters;

public interface IStep2Decoder
{
    IChart Decode(Step2Chunk chunk);
}