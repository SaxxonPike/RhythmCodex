using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Iso.Model;

[Model]
public class IsoStorageMedium
{
    public List<IsoVolume> Volumes { get; set; }
    public List<IsoBootRecord> BootRecords { get; set; }
}