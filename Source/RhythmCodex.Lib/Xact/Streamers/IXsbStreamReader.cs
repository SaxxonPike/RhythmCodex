using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXsbStreamReader
    {
        XsbFile Read(Stream stream, long length);
    }
}