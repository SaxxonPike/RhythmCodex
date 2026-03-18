using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Cd.Streamers;

namespace RhythmCodex.FileSystems.Cd.Helpers;

public static class CdSectorCollectionExtensions
{
    extension(ICdSectorCollection sectors)
    {
        /// <summary>
        /// Wraps a cache around a collection of CD sectors. If the
        /// collection is already cached, it is returned as-is.
        /// </summary>
        public ICdSectorCollection Cached()
        {
            if (sectors is CachedCdSectorCollection cached)
                return cached;
            return new CachedCdSectorCollection(sectors);
        }

        /// <summary>
        /// Creates a new collection of CD sectors from a subset of a CD sector collection.
        /// </summary>
        public ICdSectorCollection GetRange(int start) =>
            new CdSectorRange(sectors, start, sectors.Count - start);

        /// <summary>
        /// Creates a new collection of CD sectors from a subset of a CD sector collection.
        /// </summary>
        public ICdSectorCollection GetRange(int start, int count) =>
            new CdSectorRange(sectors, start, count);
    }
}