namespace RhythmCodex.Infrastructure
{
    public interface IFileAccessorFactory
    {
        IFileAccessor Create(string basePath);
    }
}