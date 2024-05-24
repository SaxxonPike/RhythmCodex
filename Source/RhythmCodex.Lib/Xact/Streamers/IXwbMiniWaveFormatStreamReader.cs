using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers;

public interface IXwbMiniWaveFormatStreamReader
{
    XwbMiniWaveFormat Read(Stream source);
}