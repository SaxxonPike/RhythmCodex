using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXwbStreamReader
    {
        IList<ISound> Read(Stream source);
    }
}