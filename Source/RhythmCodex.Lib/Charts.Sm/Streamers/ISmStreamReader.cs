using System.Collections.Generic;
using System.IO;
using RhythmCodex.Charts.Sm.Model;

namespace RhythmCodex.Charts.Sm.Streamers;

public interface ISmStreamReader
{
    List<Command> Read(Stream source);
}