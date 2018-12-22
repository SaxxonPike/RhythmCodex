using System.IO;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Streamers
{
    public interface IXa2StreamReader
    {
        Xa2Container Read(Stream stream);
    }
}