using System.IO;

namespace RhythmCodex.Infrastructure
{
    public class FileSystem : IFileSystem
    {
        public string GetFileName(string path) => Path.GetFileName(path);
        public Stream OpenRead(string path) => File.OpenRead(path);
        public Stream OpenWrite(string path) => File.OpenWrite(path);
        public string CombinePath(params string[] paths) => Path.Combine(paths);
        public string CurrentPath => Directory.GetCurrentDirectory();
        public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);
        public void WriteAllBytes(string path, byte[] data) => File.WriteAllBytes(path, data);
    }
}