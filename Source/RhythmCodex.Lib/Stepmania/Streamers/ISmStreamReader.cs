using System.Collections.Generic;
using System.IO;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Streamers;

public interface ISmStreamReader
{
    List<Command> Read(Stream source);
}