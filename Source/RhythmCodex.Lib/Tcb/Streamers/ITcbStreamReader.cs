using System.IO;
using RhythmCodex.Tcb.Models;

namespace RhythmCodex.Tcb.Streamers;

public interface ITcbStreamReader
{
    TcbImage? Read(Stream stream);
}