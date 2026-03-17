using System.Collections.Generic;
using System.IO;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Cd.Streamers;
using RhythmCodex.FileSystems.Iso.Processors;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Iso.Streamers;

[Service]
public sealed class IsoSectorCollectionFactory(IIsoSectorExpander isoSectorExpander)
    : IIsoSectorCollectionFactory
{
    public ICdSectorCollection Read(Stream stream, long length)
    {
        var collection = new IsoCdSectorCollection(stream, isoSectorExpander);
        return new CachedCdSectorCollection(collection);
    }
}