using System.IO;
using RhythmCodex.Vtddd.Models;

namespace RhythmCodex.Vtddd.Streamers;

public interface IVtdddDanceXmlStreamReader
{
    VtdddDanceDb Read(Stream stream, string chartPrefix);
}