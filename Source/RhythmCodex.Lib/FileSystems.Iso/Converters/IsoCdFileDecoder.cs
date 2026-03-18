using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.FileSystems.Cd.Helpers;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.FileSystems.Iso.Model;
using RhythmCodex.FileSystems.Iso.Streamers;
using RhythmCodex.IoC;

// ReSharper disable PossibleMultipleEnumeration

namespace RhythmCodex.FileSystems.Iso.Converters;

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
    public List<ICdFile> Decode(ICdSectorCollection cdSectors)
    {
        return DecodeInternal(cdSectors).ToList();
    }

    private IEnumerable<ICdFile> DecodeInternal(ICdSectorCollection cdSectors)
    {
        var sectorInfos = isoDescriptorSectorFinder
            .Find(cdSectors.Select(isoSectorInfoDecoder.Decode));

        var storageMediums = isoStorageMediumDecoder.Decode(sectorInfos);
        foreach (var volume in storageMediums.Volumes)
        {
            var pathLba = volume.TypeLPathTableLocation;
            var pathSector = cdSectors.TakeWhile(cds => cds.Number != pathLba).Count();
            var pathTable = isoPathTableDecoder.Decode(cdSectors.GetRange(pathSector));

            foreach (var path in pathTable)
            {
                var directorySector = cdSectors.TakeWhile(cds => cds.Number != path.LocationOfExtent).Count();
                var directory = isoDirectoryTableDecoder.Decode(cdSectors.GetRange(directorySector));

                foreach (var entry in directory)
                {
                    if (entry.Flags.HasFlag(IsoFileFlags.Directory))
                        continue;

                    yield return new CdFile(OpenFormatted, OpenRaw)
                    {
                        Name = GetPath(pathTable, entry, path),
                        Length = entry.DataLength
                    };

                    continue;

                    Stream OpenRaw()
                    {
                        var extentSector = cdSectors.TakeWhile(cds => cds.Number != entry.LocationOfExtent).Count();

                        return isoSectorStreamFactory.OpenRaw(
                            cdSectors.GetRange(extentSector),
                            (entry.DataLength + CdSector.CookedSectorSize - 1) / CdSector.CookedSectorSize * 2352);
                    }

                    Stream OpenFormatted()
                    {
                        var extentSector = cdSectors.TakeWhile(cds => cds.Number != entry.LocationOfExtent).Count();

                        return isoSectorStreamFactory.Open(
                            cdSectors.GetRange(extentSector),
                            entry.DataLength);
                    }
                }
            }
        }
    }

    private static string GetPath(IList<IsoPathRecord> pathRecords, IsoDirectoryRecord directoryRecord,
        IsoPathRecord pathRecord)
    {
        var currentPath = pathRecord;
        var output = new List<string> { directoryRecord.Identifier };

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