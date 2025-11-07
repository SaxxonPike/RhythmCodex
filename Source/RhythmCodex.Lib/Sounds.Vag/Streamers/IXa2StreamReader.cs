using System.IO;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Sounds.Vag.Streamers;

public interface IXa2StreamReader
{
    Xa2Container Read(Stream stream);
}