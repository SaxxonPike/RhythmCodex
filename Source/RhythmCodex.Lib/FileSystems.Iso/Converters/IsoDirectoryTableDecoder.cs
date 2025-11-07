using System.Collections.Generic;
using System.Linq;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Iso.Model;
using RhythmCodex.FileSystems.Iso.Streamers;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Iso.Converters;

[Service]
public class IsoDirectoryTableDecoder(
    IIsoSectorStreamFactory isoSectorStreamFactory,
    IIsoDirectoryRecordDecoder isoDirectoryRecordDecoder)
    : IIsoDirectoryTableDecoder
{
    public List<IsoDirectoryRecord> Decode(IEnumerable<ICdSector> sectors)
    {
        return DecodeInternal(sectors).ToList();
    }

    private IEnumerable<IsoDirectoryRecord> DecodeInternal(IEnumerable<ICdSector> sectors)
    {
        using var stream = isoSectorStreamFactory.Open(sectors);
        while (true)
        {
            var record = isoDirectoryRecordDecoder.Decode(stream, false);
            if (record != null)
                yield return record;
            else
                yield break;
        }
    }
}