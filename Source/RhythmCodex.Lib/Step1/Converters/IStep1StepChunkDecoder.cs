using System.Collections.Generic;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Step1.Converters;

public interface IStep1StepChunkDecoder
{
    List<Step> Convert(byte[] data);
}