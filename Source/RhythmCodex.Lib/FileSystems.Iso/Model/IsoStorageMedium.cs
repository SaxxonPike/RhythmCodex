using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.FileSystems.Iso.Model;

[Model]
public class IsoStorageMedium
{
    public List<IsoVolume> Volumes { get; set; }
    public List<IsoBootRecord> BootRecords { get; set; }
}