using System.Collections.Generic;
using System.IO;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Iso.Streamers;

public interface IIsoSectorCollectionFactory
{
    ICdSectorCollection Read(Stream stream, long length);
}