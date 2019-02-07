using System;

namespace RhythmCodex.Iso.Model
{
    [Flags]
    public enum IsoFileFlags
    {
        Hidden = 0x01,
        Directory = 0x02,
        Associated = 0x04,
        HasExtended = 0x08,
        HasPermissions = 0x10,
        LongEntry = 0x80
    }
}