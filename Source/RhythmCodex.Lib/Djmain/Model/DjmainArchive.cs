using System.Collections.Generic;
using RhythmCodex.Charting;

namespace RhythmCodex.Djmain.Model
{
    public class DjmainArchive : IDjmainArchive
    {
        public int Id { get; set; }
        public IList<IChart> Charts { get; set; }
        public IList<IDjmainSample> Samples { get; set; }
    }
}