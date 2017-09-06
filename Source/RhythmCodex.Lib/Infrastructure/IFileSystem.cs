using System.IO;

namespace RhythmCodex.Infrastructure
{
    public interface IFileSystem
    {
        string GetFileName(string path);
        Stream OpenRead(string path);
        Stream OpenWrite(string path);
        string CombinePath(params string[] paths);
        string CurrentPath { get; }
        byte[] ReadAllBytes(string path);
        void WriteAllBytes(string path, byte[] data);
    }
}