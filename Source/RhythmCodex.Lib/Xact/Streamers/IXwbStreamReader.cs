using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXwbStreamReader
    {
        IEnumerable<ISound> Read(Stream source);
    }
}