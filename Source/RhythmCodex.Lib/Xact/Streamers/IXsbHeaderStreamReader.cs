using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXsbHeaderStreamReader
    {
        XsbHeader Read(Stream stream);
    }
}