using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Iso.Model
{
    [Model]
    public class Iso9660StorageMedium
    {
        public IList<Iso9660Volume> Volumes { get; set; }
        public IList<Iso9660BootRecord> BootRecords { get; set; }
    }
}