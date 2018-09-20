using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Iso.Model
{
    [Model]
    public class IsoStorageMedium
    {
        public IList<IsoVolume> Volumes { get; set; }
        public IList<IsoBootRecord> BootRecords { get; set; }
    }
}