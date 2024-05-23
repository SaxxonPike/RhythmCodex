using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers;

public interface IXsbHeaderStreamWriter
{
    int Write(Stream stream, XsbHeader header);
}