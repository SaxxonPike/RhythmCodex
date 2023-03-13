using RhythmCodex.Infrastructure;

namespace RhythmCodex.Chd.Model;

[Model]
public struct ChdHeaderInfo
{
    public uint compression;
    public uint[] compressors;
    public uint cylinders;
    public uint flags;
    public uint heads;
    public uint hunkBytes;
    public uint hunkSize;
    public ulong logicalBytes;
    public ulong mapOffset;
    public byte[] md5;
    public ulong metaOffset;
    public byte[] parentmd5;
    public byte[] parentsha1;
    public byte[] rawsha1;
    public uint seclen;
    public uint sectors;
    public byte[] sha1;
    public uint totalHunks;
    public uint unitBytes;
}