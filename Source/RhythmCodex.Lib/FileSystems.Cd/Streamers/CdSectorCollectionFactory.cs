using System.Collections.Generic;
using System.IO;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Cd.Streamers;

[Service]
public class CdSectorCollectionFactory : ICdSectorCollectionFactory
{
    public ICdSectorCollection Create(Stream stream, long length)
    {
        var collection = new StreamCdSectorCollection(stream, length);
        return new CachedCdSectorCollection(collection);
    }
}