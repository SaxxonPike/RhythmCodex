using System.IO;

namespace RhythmCodex.Streamers
{
    public interface IStreamWriter<in TData>
    {
        void Write(Stream stream, TData data);
    }
}
