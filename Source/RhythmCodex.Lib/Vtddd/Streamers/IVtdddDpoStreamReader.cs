using System.IO;
using RhythmCodex.Vtddd.Models;

namespace RhythmCodex.Vtddd.Streamers
{
    public interface IVtdddDpoStreamReader
    {
        VtdddDpoFile Read(Stream stream, int length);
    }
}