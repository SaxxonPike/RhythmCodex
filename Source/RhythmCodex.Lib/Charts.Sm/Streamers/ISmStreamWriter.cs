using System.Collections.Generic;
using System.IO;
using RhythmCodex.Charts.Sm.Model;

namespace RhythmCodex.Charts.Sm.Streamers;

public interface ISmStreamWriter
{
    void Write(Stream stream, IEnumerable<Command> commands);
}