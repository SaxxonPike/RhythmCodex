using System.Collections.Generic;
using System.IO;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Cd.Streamers;

public interface ICdSectorStreamReader
{
    IEnumerable<ICdSector> Read(Stream stream, long length, bool keepOnDisk, int sectorLength = 2352);
}