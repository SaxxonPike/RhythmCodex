using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Heuristics;

public interface IDjmainHddDescriptionHeuristic
{
    DjmainHddDescription Get(byte[] chunk);
}