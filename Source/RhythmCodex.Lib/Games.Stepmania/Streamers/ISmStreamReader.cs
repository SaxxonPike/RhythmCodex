using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Stepmania.Model;

namespace RhythmCodex.Games.Stepmania.Streamers;

public interface ISmStreamReader
{
    List<Command> Read(Stream source);
}