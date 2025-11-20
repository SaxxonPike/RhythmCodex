using System.IO;
using RhythmCodex.Games.Vtddd.Models;

namespace RhythmCodex.Games.Vtddd.Streamers;

public interface IVtdddDanceXmlStreamReader
{
    VtdddDanceDb Read(Stream stream, string chartPrefix);
}