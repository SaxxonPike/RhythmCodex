using System.IO;
using RhythmCodex.Graphics.Tcb.Models;

namespace RhythmCodex.Graphics.Tcb.Streamers;

public interface ITcbStreamReader
{
    TcbImage? Read(Stream stream);
}