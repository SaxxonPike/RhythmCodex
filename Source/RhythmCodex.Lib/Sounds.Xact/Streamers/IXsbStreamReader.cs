using System.IO;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

public interface IXsbStreamReader
{
    XsbFile Read(Stream stream, long length);
}