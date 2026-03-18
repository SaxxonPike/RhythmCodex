using System.IO;
using RhythmCodex.FileSystems.Cd.Helpers;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Cd.Streamers;

[Service]
public class CdSectorCollectionFactory : ICdSectorCollectionFactory
{
    public ICdSectorCollection Create(Stream stream, long length) =>
        new StreamCdSectorCollection(stream, length).Cached();
}