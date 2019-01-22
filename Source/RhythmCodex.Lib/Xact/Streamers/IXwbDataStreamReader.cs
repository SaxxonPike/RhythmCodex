using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXwbDataStreamReader
    {
        XwbData Read(Stream source);
    }
}