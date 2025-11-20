using System.IO;
using RhythmCodex.Games.Vtddd.Models;

namespace RhythmCodex.Games.Vtddd.Streamers;

public interface IVtdddDpoStreamReader
{
    VtdddDpoFile? Read(Stream stream, int length);
}