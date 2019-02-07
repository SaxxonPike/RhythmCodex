using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Iso.Model
{
    [Model]
    public class Iso9660PathTableEntry
    {
        public int ExtendedAttributeRecordLength { get; set; }
        public int LocationOfExtent { get; set; }
        public int ParentDirectoryNumber { get; set; }
        public string DirectoryIdentifier { get; set; }
    }
}