using RhythmCodex.Infrastructure;

namespace RhythmCodex.Chd.Model;

[Model]
public struct ChdMapInfo
{
    public uint compression;
    public uint crc32;
    public byte flags;
    public ulong length;
    public ulong offset;
}