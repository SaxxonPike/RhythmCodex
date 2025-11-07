using System;
using System.IO;

namespace RhythmCodex.FileSystems.Cd.Model;

public class CdFile(Func<Stream> openFunc) : ICdFile
{
    public string? Name { get; set; }
    public long Length { get; set; }

    public Stream Open() => openFunc();
}