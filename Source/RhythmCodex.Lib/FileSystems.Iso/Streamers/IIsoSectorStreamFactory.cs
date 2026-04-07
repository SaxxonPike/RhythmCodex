using System.IO;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Iso.Streamers;

public interface IIsoSectorStreamFactory
{
    Stream Open(ICdSectorCollection sectors);
    Stream Open(ICdSectorCollection sectors, long length);
    Stream OpenRaw(ICdSectorCollection sectors);
    Stream OpenRaw(ICdSectorCollection sectors, long length);
}