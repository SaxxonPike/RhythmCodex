using System.IO;

namespace RhythmCodex.Cd.Model
{
    public interface ICdFile
    {
        string Name { get; }
        long Length { get; }
        Stream Open();
    }
}