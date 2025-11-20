using System.IO;
using RhythmCodex.Graphics.Tim.Models;

namespace RhythmCodex.Graphics.Tim.Streamers;

public interface ITimStreamReader
{
    TimImage Read(Stream stream);
}