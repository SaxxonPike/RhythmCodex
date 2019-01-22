using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXwbStreamReader
    {
        IEnumerable<XwbSound> Read(Stream source);
    }
}