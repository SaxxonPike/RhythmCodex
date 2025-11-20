using System.IO;
using RhythmCodex.Charts.Step2.Models;

namespace RhythmCodex.Charts.Step2.Streamers;

public interface IStep2StreamReader
{
    Step2Chunk Read(Stream stream, int length);
}