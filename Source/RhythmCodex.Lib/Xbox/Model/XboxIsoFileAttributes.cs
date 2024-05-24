using System;

namespace RhythmCodex.Xbox.Model;

[Flags]
public enum XboxIsoFileAttributes
{
    ReadOnly = 0x01,
    Hidden = 0x02,
    System = 0x04,
    Directory = 0x10,
    Archive = 0x20,
    Nor = 0x80
}