using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Iso.Model;

[Model]
public class IsoDirectoryRecord
{
    public int ExtendedAttributeRecordLength { get; set; }
    public int LocationOfExtent { get; set; }
    public long DataLength { get; set; }
    public DateTimeOffset? RecordingDateTime { get; set; }
    public IsoFileFlags Flags { get; set; }
    public int UnitSize { get; set; }
    public int InterleaveGapSize { get; set; }
    public int VolumeSequenceNumber { get; set; }
    public string Identifier { get; set; }
    public byte[] Extra { get; set; }
}