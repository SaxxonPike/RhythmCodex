using System.IO;
using RhythmCodex.Step2.Models;

namespace RhythmCodex.Step2.Streamers;

public interface IStep2StreamReader
{
    Step2Chunk Read(Stream stream, int length);
}