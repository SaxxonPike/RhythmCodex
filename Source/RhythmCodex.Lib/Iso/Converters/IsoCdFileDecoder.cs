using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Cd.Model;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Model;
using RhythmCodex.Iso.Streamers;

// ReSharper disable PossibleMultipleEnumeration

namespace RhythmCodex.Iso.Converters;

[Service]
public class IsoCdFileDecoder : IIsoCdFileDecoder
{
    private readonly IIsoStorageMediumDecoder _isoStorageMediumDecoder;
    private readonly IIsoSectorInfoDecoder _isoSectorInfoDecoder;
    private readonly IIsoDescriptorSectorFinder _isoDescriptorSectorFinder;
    private readonly IIsoPathTableDecoder _isoPathTableDecoder;
    private readonly IIsoSectorStreamFactory _isoSectorStreamFactory;
    private readonly IIsoDirectoryTableDecoder _isoDirectoryTableDecoder;

    public IsoCdFileDecoder(
        IIsoStorageMediumDecoder isoStorageMediumDecoder,
        IIsoSectorInfoDecoder isoSectorInfoDecoder,
        IIsoDescriptorSectorFinder isoDescriptorSectorFinder,
        IIsoPathTableDecoder isoPathTableDecoder,
        IIsoSectorStreamFactory isoSectorStreamFactory,
        IIsoDirectoryTableDecoder isoDirectoryTableDecoder)
    {
        _isoStorageMediumDecoder = isoStorageMediumDecoder;
        _isoSectorInfoDecoder = isoSectorInfoDecoder;
        _isoDescriptorSectorFinder = isoDescriptorSectorFinder;
        _isoPathTableDecoder = isoPathTableDecoder;
        _isoSectorStreamFactory = isoSectorStreamFactory;
        _isoDirectoryTableDecoder = isoDirectoryTableDecoder;
    }
        
    public IList<ICdFile> Decode(IEnumerable<ICdSector> cdSectors)
    {
        return DecodeInternal(cdSectors).ToList();
    }

    private IEnumerable<ICdFile> DecodeInternal(IEnumerable<ICdSector> cdSectors)
    {
        var sectorInfos = _isoDescriptorSectorFinder
            .Find(cdSectors.Select(s => _isoSectorInfoDecoder.Decode(s)));

        var storageMediums = _isoStorageMediumDecoder.Decode(sectorInfos);
        foreach (var volume in storageMediums.Volumes)
        {
            var pathLba = volume.TypeLPathTableLocation;
            var pathTable = _isoPathTableDecoder.Decode(cdSectors.SkipWhile(cds => cds.Number != pathLba));
                
            foreach (var path in pathTable)
            {
                var directory =
                    _isoDirectoryTableDecoder.Decode(
                        cdSectors.SkipWhile(cds => cds.Number != path.LocationOfExtent));
                foreach (var entry in directory)
                {
                    if (entry.Flags.HasFlag(IsoFileFlags.Directory))
                        continue;
                        
                    yield return new CdFile(() =>
                        _isoSectorStreamFactory.Open(
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