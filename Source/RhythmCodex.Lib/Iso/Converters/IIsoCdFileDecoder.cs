using System.Collections.Generic;
using RhythmCodex.Cd.Model;

namespace RhythmCodex.Iso.Converters;

public interface IIsoCdFileDecoder
{
    List<ICdFile> Decode(IEnumerable<ICdSector> cdSectors);
}