namespace RhythmCodex.Infrastructure.Converters
{
    public interface ISlicer
    {
        T[] Slice<T>(T[] arr, int offset, int length);
    }
}