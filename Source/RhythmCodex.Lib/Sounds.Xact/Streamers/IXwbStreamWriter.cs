using System.Collections.Generic;
using System.IO;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

public interface IXwbStreamWriter
{
    int Write(Stream target, IEnumerable<XwbSound> sounds);
}