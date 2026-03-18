using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Cd.Streamers;

namespace RhythmCodex.FileSystems.Cd.Helpers;

public static class CdSectorCollectionExtensions
{
    extension(ICdSectorCollection sectors)
    {
        public ICdSectorCollection GetRange(int start) =>
            new CdSectorRange(sectors, start, sectors.Count - start);

        public ICdSectorCollection GetRange(int start, int count) =>
            new CdSectorRange(sectors, start, count);
    }
}