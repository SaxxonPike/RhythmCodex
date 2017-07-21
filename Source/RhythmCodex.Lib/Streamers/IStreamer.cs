using System.IO;

namespace RhythmCodex.Streamers
{
    public interface IStreamer<TData>
    {
        TData Read(Stream stream);
        void Write(Stream stream, TData data);
    }
}
