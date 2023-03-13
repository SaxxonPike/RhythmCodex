using System.Collections.Generic;
using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers;

public interface IXwbStreamWriter
{
    int Write(Stream target, IEnumerable<XwbSound> sounds);
}