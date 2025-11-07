using System.IO;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

public interface IXwbHeaderStreamReader
{
    XwbHeader Read(Stream source);
}