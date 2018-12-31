using System.IO;

namespace RhythmCodex.Infrastructure
{
    public interface IFileAccessor
    {
        bool FileExists(string name);
        Stream OpenRead(string name);
    }
}