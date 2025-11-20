using System.Collections.Generic;
using System.IO;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

public interface IXwbStreamReader
{
    IEnumerable<XwbSound> Read(Stream source);
}