using System.Collections.Generic;
using System.IO;
using RhythmCodex.Step1.Models;

namespace RhythmCodex.Step1.Streamers;

public interface IStep1StreamReader
{
    List<Step1Chunk> Read(Stream stream);
}