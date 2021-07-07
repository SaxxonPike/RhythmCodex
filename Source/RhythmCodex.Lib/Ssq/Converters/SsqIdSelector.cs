using RhythmCodex.Charting.Models;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class SsqIdSelector : ISsqIdSelector
    {
        public int? SelectType(Chart chart)
        {
            var type = chart[StringData.Type]?.ToLower();
            if (type == SmGameTypes.DanceSingle.ToLower())
                return 0x0014;
            if (type == SmGameTypes.DanceSolo.ToLower())
                return 0x0016;
            if (type == SmGameTypes.DanceDouble.ToLower())
                return 0x0018;
            if (type == SmGameTypes.DanceCouple.ToLower())
                return 0x0024;
            return null;
        }
        
        public int? SelectDifficulty(Chart chart)
        {
            var difficulty = chart[StringData.Difficulty]?.ToLower();
            if (chart[StringData.Type]?.ToLower() == SmGameTypes.DanceCouple.ToLower())
                return 0x1000; // todo: wtf? why does couple use a different difficulty??
            if (difficulty == SmNotesDifficulties.Easy.ToLower())
                return 0x0100;
            if (difficulty == SmNotesDifficulties.Medium.ToLower())
                return 0x0200;
            if (difficulty == SmNotesDifficulties.Hard.ToLower())
                return 0x0300;
            if (difficulty == SmNotesDifficulties.Beginner.ToLower())
                return 0x0400;
            if (difficulty == SmNotesDifficulties.Challenge.ToLower())
                return 0x0600;
            return null;
        }
    }
}