using System.Collections.Generic;
using System.IO;
using RhythmCodex.Cd.Model;

namespace RhythmCodex.Cd.Streamers;

public interface ICdSectorStreamReader
{
    IEnumerable<ICdSector> Read(Stream stream, long length, bool keepOnDisk, int sectorLength = 2352);
}