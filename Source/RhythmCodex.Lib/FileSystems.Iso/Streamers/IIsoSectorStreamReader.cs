using System.Collections.Generic;
using System.IO;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Iso.Streamers;

public interface IIsoSectorStreamReader
{
    IEnumerable<ICdSector> Read(Stream stream, int length, bool keepOnDisk);
}