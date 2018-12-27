using System;

namespace RhythmCodex.Dds.Models
{
    [Flags]
    public enum DdsCaps1
    {
        DDSCAPS_COMPLEX = 0x8,
        DDSCAPS_MIPMAP = 0x400000,
        DDSCAPS_TEXTURE = 0x1000
    }
}