using System.IO;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Iso.Streamers;

public interface IIsoSectorCollectionFactory
{
    ICdSectorCollection Create(Stream stream, long length);
}