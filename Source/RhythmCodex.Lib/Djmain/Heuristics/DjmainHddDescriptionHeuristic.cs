using System.Collections.Generic;
using System.Text;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Heuristics
{
    [Service]
    public class DjmainHddDescriptionHeuristic : IDjmainHddDescriptionHeuristic
    {
        public DjmainHddDescription Get(byte[] chunk)
        {
            DjmainChunkFormat GetFormat(string formatId)
            {
                switch (formatId)
                {
                    case @"GQ753": return DjmainChunkFormat.First;
                    case @"GX853": return DjmainChunkFormat.Second;
                    case @"GQ825": return DjmainChunkFormat.Third;
                    case @"GQ847": return DjmainChunkFormat.Fourth;
                    case @"GQ981": return DjmainChunkFormat.Fifth;
                    case @"GCA21": return DjmainChunkFormat.Sixth;
                    case @"GEB07": return DjmainChunkFormat.Seventh;
                    case @"GQ993": return DjmainChunkFormat.Club;
                    case @"GQ858": return DjmainChunkFormat.Complete;
                    case @"GQ988": return DjmainChunkFormat.Complete2;
                    case @"GQA05": return DjmainChunkFormat.Core;
                    case @"GQ995": return DjmainChunkFormat.Dct;
                    case @"GCC01": return DjmainChunkFormat.Final;
                    default: return DjmainChunkFormat.Unknown;
                }
            }

            var format =
                GetFormat(Encoding.ASCII.GetString(new[] {chunk[1], chunk[0], chunk[3], chunk[2], chunk[5]}));
            if (format != DjmainChunkFormat.Unknown)
                return new DjmainHddDescription
                {
                    Format = format,
                    BytesAreSwapped = false
                };

            format = 
                GetFormat(Encoding.ASCII.GetString(new[] {chunk[0], chunk[1], chunk[2], chunk[3], chunk[4]}));
            if (format != DjmainChunkFormat.Unknown)
                return new DjmainHddDescription
                {
                    Format = format,
                    BytesAreSwapped = true
                };
            
            return new DjmainHddDescription
            {
                Format = DjmainChunkFormat.Unknown
            };
        }
    }
}