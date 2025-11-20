using System;
using RhythmCodex.Archs.Djmain.Model;

namespace RhythmCodex.Archs.Djmain.Heuristics;

public interface IDjmainHddDescriptionHeuristic
{
    DjmainHddDescription Get(ReadOnlySpan<byte> chunk);
}