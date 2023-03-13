using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers;

public interface IXsbStreamWriter
{
    long Write(Stream stream, XsbFile file);
}