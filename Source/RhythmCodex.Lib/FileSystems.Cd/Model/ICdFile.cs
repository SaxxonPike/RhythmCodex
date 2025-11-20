using System.IO;

namespace RhythmCodex.FileSystems.Cd.Model;

public interface ICdFile
{
    string? Name { get; }
    long Length { get; }
    Stream Open();
}