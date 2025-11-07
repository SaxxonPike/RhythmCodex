using System.IO;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

public interface IXwbDataStreamReader
{
    XwbData Read(Stream source);
}