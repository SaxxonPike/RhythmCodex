using System.IO;

namespace RhythmCodex.Streamers
{
    public interface IStreamReader<out TData>
    {
        TData Read(Stream stream);
    }
}
