using RhythmCodex.Infrastructure;

namespace RhythmCodex.FileSystems.Iso.Model;

[Model]
public class IsoPathRecord
{
    public int ExtendedAttributeRecordLength { get; set; }
    public int LocationOfExtent { get; set; }
    public int ParentDirectoryNumber { get; set; }
    public string DirectoryIdentifier { get; set; }
}