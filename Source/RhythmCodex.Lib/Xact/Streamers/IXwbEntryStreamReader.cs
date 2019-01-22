using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXwbEntryStreamReader
    {
        XwbEntry Read(Stream source);
    }
}