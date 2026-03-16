using System;
using System.IO;

namespace RhythmCodex.FileSystems.Cd.Model;

public class CdFile(Func<Stream> openFunc, Func<Stream> openRawFunc) : ICdFile
{
    public string? Name { get; set; }
    public long Length { get; set; }

    public Stream Open() => openFunc();
    public Stream OpenRaw() => openRawFunc();
}