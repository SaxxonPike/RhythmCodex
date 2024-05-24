using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Iso.Model;

[Model]
public class IsoBootRecord
{
    public string BootSystemIdentifier { get; set; }
    public string BootIdentifier { get; set; }
    public Memory<byte> BootSystemData { get; set; }
}