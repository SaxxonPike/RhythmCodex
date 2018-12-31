namespace RhythmCodex.Infrastructure
{
    [Service]
    public class FileAccessorFactory : IFileAccessorFactory
    {
        public IFileAccessor Create(string basePath)
        {
            return new FileAccessor(basePath);
        }
    }
}