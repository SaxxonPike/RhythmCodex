using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters;

[Service]
public class IsoStorageMediumDecoder(
    IIsoPrimaryVolumeDescriptorDecoder isoPrimaryVolumeDescriptorDecoder,
    IIsoBootRecordDecoder isoBootRecordDecoder,
    IIsoDescriptorSectorFinder isoDescriptorSectorFinder)
    : IIsoStorageMediumDecoder
{
    public IsoStorageMedium Decode(IEnumerable<IsoSectorInfo> sectors)
    {
        var result = new IsoStorageMedium
        {
            BootRecords = new List<IsoBootRecord>(),
            Volumes = new List<IsoVolume>()
        };

        var descriptorSectors = isoDescriptorSectorFinder.Find(sectors).ToList();
            
        var bootDescriptors = descriptorSectors.Where(s => s.UserData[0] == 0x00).ToList();
        var primaryVolumeDescriptor = descriptorSectors.Single(s => s.UserData[0] == 0x01);
//            var supplementaryVolumeDescriptors = descriptorSectors.Where(s => s.UserData[0] == 0x02).ToList();
//            var partitionDescriptors = descriptorSectors.Where(s => s.UserData[0] == 0x03).ToList();

        foreach (var bootDescriptor in bootDescriptors)
            result.BootRecords.Add(isoBootRecordDecoder.Decode(bootDescriptor.UserData));

        var primaryVolume = isoPrimaryVolumeDescriptorDecoder.Decode(primaryVolumeDescriptor.UserData);
            
        result.Volumes.Add(primaryVolume);

        return result;
    }
        
        
}