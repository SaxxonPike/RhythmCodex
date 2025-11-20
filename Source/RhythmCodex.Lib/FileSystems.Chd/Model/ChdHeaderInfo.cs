using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.FileSystems.Chd.Model;

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
    public Memory<byte> md5;
    public ulong metaOffset;
    public Memory<byte> parentmd5;
    public Memory<byte> parentsha1;
    public Memory<byte> rawsha1;
    public uint seclen;
    public uint sectors;
    public Memory<byte> sha1;
    public uint totalHunks;
    public uint unitBytes;
}