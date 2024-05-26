using RhythmCodex.Djmain.Model;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;

namespace RhythmCodex.Djmain.Heuristics;

[Service]
public class DjmainHddDescriptionHeuristic : IDjmainHddDescriptionHeuristic
{
    public DjmainHddDescription Get(byte[] chunk)
    {
        var format =
            GetFormat(new[] {chunk[1], chunk[0], chunk[3], chunk[2], chunk[5], chunk[4]}.GetString());
        if (format != DjmainChunkFormat.Unknown)
            return new DjmainHddDescription
            {
                Format = format,
                BytesAreSwapped = false
            };

        format = 
            GetFormat(new[] {chunk[0], chunk[1], chunk[2], chunk[3], chunk[4], chunk[5]}.GetString());
        if (format != DjmainChunkFormat.Unknown)
            return new DjmainHddDescription
            {
                Format = format,
                BytesAreSwapped = true
            };
            
        return new DjmainHddDescription
        {
            Format = DjmainChunkFormat.Unknown,
            BytesAreSwapped = false
        };

        DjmainChunkFormat GetFormat(string formatId)
        {
            switch (formatId)
            {
                case " GQ753": return DjmainChunkFormat.BeatmaniaFirst;
                case " GX853": return DjmainChunkFormat.BeatmaniaSecond;
                case " GQ825": return DjmainChunkFormat.BeatmaniaThird;
                case " GQ847": return DjmainChunkFormat.BeatmaniaFourth;
                case " GQ981": return DjmainChunkFormat.BeatmaniaFifth;
                case " GCA21": return DjmainChunkFormat.BeatmaniaSixth;
                case " GEB07": return DjmainChunkFormat.BeatmaniaSeventh;
                case " GQ993": return DjmainChunkFormat.BeatmaniaClub;
                case " GQ858": return DjmainChunkFormat.BeatmaniaComplete;
                case " GQ988": return DjmainChunkFormat.BeatmaniaComplete2;
                case " GQA05": return DjmainChunkFormat.BeatmaniaCore;
                case " GQ995": return DjmainChunkFormat.BeatmaniaDct;
                case " GCC01": return DjmainChunkFormat.BeatmaniaFinal;
                default: return DjmainChunkFormat.Unknown;
            }
        }
    }
}