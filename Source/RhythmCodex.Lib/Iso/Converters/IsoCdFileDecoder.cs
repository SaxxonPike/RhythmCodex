using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Cd.Model;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Model;
using RhythmCodex.Iso.Streamers;

// ReSharper disable PossibleMultipleEnumeration

namespace RhythmCodex.Iso.Converters;

[Service]
public class IsoCdFileDecoder(
    IIsoStorageMediumDecoder isoStorageMediumDecoder,
    IIsoSectorInfoDecoder isoSectorInfoDecoder,
    IIsoDescriptorSectorFinder isoDescriptorSectorFinder,
    IIsoPathTableDecoder isoPathTableDecoder,
    IIsoSectorStreamFactory isoSectorStreamFactory,
    IIsoDirectoryTableDecoder isoDirectoryTableDecoder)
    : IIsoCdFileDecoder
{
    public List<ICdFile> Decode(IEnumerable<ICdSector> cdSectors)
    {
        return DecodeInternal(cdSectors).ToList();
    }

    private IEnumerable<ICdFile> DecodeInternal(IEnumerable<ICdSector> cdSectors)
    {
        var sectorInfos = isoDescriptorSectorFinder
            .Find(cdSectors.Select(s => isoSectorInfoDecoder.Decode(s)));

        var storageMediums = isoStorageMediumDecoder.Decode(sectorInfos);
        foreach (var volume in storageMediums.Volumes)
        {
            var pathLba = volume.TypeLPathTableLocation;
            var pathTable = isoPathTableDecoder.Decode(cdSectors.SkipWhile(cds => cds.Number != pathLba));
                
            foreach (var path in pathTable)
            {
                var directory =
                    isoDirectoryTableDecoder.Decode(
                        cdSectors.SkipWhile(cds => cds.Number != path.LocationOfExtent));
                foreach (var entry in directory)
                {
                    if (entry.Flags.HasFlag(IsoFileFlags.Directory))
                        continue;
                        
                    yield return new CdFile(() =>
                        isoSectorStreamFactory.Open(
                            cdSectors.SkipWhile(cds => cds.Number != entry.LocationOfExtent), entry.DataLength))
                    {
                        Name = GetPath(pathTable, entry, path),
                        Length = entry.DataLength
                    };
                }
            }
        }
    }

    private string GetPath(IList<IsoPathRecord> pathRecords, IsoDirectoryRecord directoryRecord, IsoPathRecord pathRecord)
    {
        var currentPath = pathRecord;
        var output = new List<string> {directoryRecord.Identifier};

        while (currentPath.ParentDirectoryNumber != 0)
        {
            output.Add(currentPath.DirectoryIdentifier);
            var newPath = pathRecords[currentPath.ParentDirectoryNumber - 1];
            if (newPath == currentPath)
                break;
            currentPath = newPath;
        }

        output.Reverse();
        return string.Join("/", output.Where(o => !string.IsNullOrEmpty(o)));
    }
}