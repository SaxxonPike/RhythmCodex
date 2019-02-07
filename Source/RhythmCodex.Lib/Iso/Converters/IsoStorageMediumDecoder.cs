using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    [Service]
    public class IsoStorageMediumDecoder : IIsoStorageMediumDecoder
    {
        private readonly IIsoPrimaryVolumeDescriptorDecoder _isoPrimaryVolumeDescriptorDecoder;
        private readonly IIsoBootRecordDecoder _isoBootRecordDecoder;
        private readonly IIsoDescriptorSectorFinder _isoDescriptorSectorFinder;

        public IsoStorageMediumDecoder(
            IIsoPrimaryVolumeDescriptorDecoder isoPrimaryVolumeDescriptorDecoder,
            IIsoBootRecordDecoder isoBootRecordDecoder,
            IIsoDescriptorSectorFinder isoDescriptorSectorFinder)
        {
            _isoPrimaryVolumeDescriptorDecoder = isoPrimaryVolumeDescriptorDecoder;
            _isoBootRecordDecoder = isoBootRecordDecoder;
            _isoDescriptorSectorFinder = isoDescriptorSectorFinder;
        }
        
        public IsoStorageMedium Decode(IEnumerable<IsoSectorInfo> sectors)
        {
            var result = new IsoStorageMedium
            {
                BootRecords = new List<IsoBootRecord>(),
                Volumes = new List<IsoVolume>()
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