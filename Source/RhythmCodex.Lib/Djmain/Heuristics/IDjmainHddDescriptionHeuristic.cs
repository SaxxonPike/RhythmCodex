using System;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Heuristics;

public interface IDjmainHddDescriptionHeuristic
{
    DjmainHddDescription Get(ReadOnlySpan<byte> chunk);
}