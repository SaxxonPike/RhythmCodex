using System.Collections.Generic;
using System.IO;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Cd.Streamers;

public interface ICdSectorCollectionFactory
{
    ICdSectorCollection Create(Stream stream, long length);
}