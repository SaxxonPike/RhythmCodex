using System.IO;

namespace RhythmCodex.Streamers
{
    public interface IStreamer<out TRead, in TWrite>
    {
        TRead Read(Stream stream);
        void Write(Stream stream, TWrite data);
    }
}
