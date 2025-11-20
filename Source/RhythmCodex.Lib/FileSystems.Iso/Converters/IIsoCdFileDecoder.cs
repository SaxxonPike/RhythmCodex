using System.Collections.Generic;
using RhythmCodex.FileSystems.Cd.Model;

namespace RhythmCodex.FileSystems.Iso.Converters;

public interface IIsoCdFileDecoder
{
    List<ICdFile> Decode(IEnumerable<ICdSector> cdSectors);
}