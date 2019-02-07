using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    [Service]
    public class CdStorageMediumDecoder : ICdStorageMediumDecoder
    {
        private readonly IIsoPrimaryVolumeDescriptorDecoder _isoPrimaryVolumeDescriptorDecoder;
        private readonly IIsoBootRecordDecoder _isoBootRecordDecoder;
        private readonly IIsoDescriptorSectorFinder _isoDescriptorSectorFinder;

        public CdStorageMediumDecoder(
            IIsoPrimaryVolumeDescriptorDecoder isoPrimaryVolumeDescriptorDecoder,
            IIsoBootRecordDecoder isoBootRecordDecoder,
            IIsoDescriptorSectorFinder isoDescriptorSectorFinder)
        {
            _isoPrimaryVolumeDescriptorDecoder = isoPrimaryVolumeDescriptorDecoder;
            _isoBootRecordDecoder = isoBootRecordDecoder;
            _isoDescriptorSectorFinder = isoDescriptorSectorFinder;
        }
        
        public Iso9660StorageMedium Decode(IEnumerable<Iso9660SectorInfo> sectors)
        {
            var result = new Iso9660StorageMedium
            {
                BootRecords = new List<Iso9660BootRecord>(),
                Volumes = new List<Iso9660Volume>()
            };

            var descriptorSectors = _isoDescriptorSectorFinder.Find(sectors).ToList();
            
            var bootDescriptors = descriptorSectors.Where(s => s.UserData[0] == 0x00).ToList();
            var primaryVolumeDescriptor = descriptorSectors.Single(s => s.UserData[0] == 0x01);
//            var supplementaryVolumeDescriptors = descriptorSectors.Where(s => s.UserData[0] == 0x02).ToList();
//            var partitionDescriptors = descriptorSectors.Where(s => s.UserData[0] == 0x03).ToList();

            foreach (var bootDescriptor in bootDescriptors)
                result.BootRecords.Add(_isoBootRecordDecoder.Decode(bootDescriptor.UserData));

            var primaryVolume = _isoPrimaryVolumeDescriptorDecoder.Decode(primaryVolumeDescriptor.UserData);
            
            result.Volumes.Add(primaryVolume);

            return result;
        }
        
        
    }
}