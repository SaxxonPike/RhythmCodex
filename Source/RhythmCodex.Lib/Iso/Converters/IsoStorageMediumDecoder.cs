using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Converters;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public class IsoStorageMediumDecoder : IIsoStorageMediumDecoder
    {
        private static readonly byte[] StandardIdentifier = {0x43, 0x44, 0x30, 0x30, 0x31};
        
        private readonly ISlicer _slicer;
        private readonly IBitter _bitter;
        private readonly IBcd _bcd;

        public IsoStorageMediumDecoder(ISlicer slicer, IBitter bitter, IBcd bcd)
        {
            _slicer = slicer;
            _bitter = bitter;
            _bcd = bcd;
        }
        
        public IList<IsoStorageMedium> Decode(IEnumerable<IsoSectorInfo> sectors)
        {
            var result = new IsoStorageMedium
            {
                BootRecords = new List<IsoBootRecord>(),
                Volumes = new List<IsoVolume>()
            };
            
            var mode1Sectors = sectors.Where(s => s.Mode == 1).ToList();
            var currentMinute = 2;
            var currentSecond = 0;
            var currentFrame = 16;
            var descriptorSectors = new List<IsoSectorInfo>();

            while (true)
            {
                var descriptorSector = mode1Sectors.SingleOrDefault(s => s.Frames == currentFrame &&
                                                                         s.Seconds == currentSecond &&
                                                                         s.Minutes == currentMinute);
                
                if (descriptorSector == null)
                    throw new RhythmCodexException($"Volume descriptors incomplete. MSF={currentMinute:000}:{currentSecond:00}:{currentFrame:00}");
                
                if (!_slicer.Slice(descriptorSector.Data, 0x0019, 5).SequenceEqual(StandardIdentifier))
                    throw new RhythmCodexException($"Standard identifier is incorrect. MSF={currentMinute:000}:{currentSecond:00}:{currentFrame:00}");

                descriptorSectors.Add(descriptorSector);
                
                if (descriptorSector.Data[0x0018] == 255)
                    break;

                currentFrame++;
                
                if (currentFrame >= 75)
                {
                    currentFrame = 0;
                    currentSecond++;
                }

                if (currentSecond >= 60)
                {
                    currentSecond = 0;
                    currentMinute++;
                }
            }

            var bootDescriptors = descriptorSectors.Where(s => s.Data[0x0018] == 0x00).ToList();
            var primaryVolumeDescriptor = descriptorSectors.Single(s => s.Data[0x0018] == 0x01);
            var supplementaryVolumeDescriptors = descriptorSectors.Where(s => s.Data[0x0018] == 0x02).ToList();
            var partitionDescriptors = descriptorSectors.Where(s => s.Data[0x0018] == 0x03).ToList();

            foreach (var bootDescriptor in bootDescriptors)
            {
                result.BootRecords.Add(new IsoBootRecord
                {
                    BootSystemIdentifier = Encodings.CP437.GetString(_slicer.Slice(bootDescriptor.Data, 0x17 + 8, 32)),
                    BootIdentifier = Encodings.CP437.GetString(_slicer.Slice(bootDescriptor.Data, 0x17 + 40, 32)),
                    BootSystemData = _slicer.Slice(bootDescriptor.Data, 0x17 + 72, 1977)
                });
            }

            var primaryVolume = new IsoVolume
            {
                SystemIdentifier = Encodings.CP437.GetString(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 9, 32)),
                VolumeIdentifier = Encodings.CP437.GetString(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 41, 32)),
                SpaceSize = _bitter.ToInt32(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 81, 4)),
                SetSize = _bitter.ToInt32(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 121, 2)),
                SequenceNumber = _bitter.ToInt32(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 125, 2)),
                LogicalBlockSize = _bitter.ToInt32(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 129, 2)),
                PathTableSize = _bitter.ToInt32(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 133, 4)),
                TypeLPathTableLocation = _bitter.ToInt32(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 141, 2)),
                OptionalTypeLPathTableLocation = _bitter.ToInt32(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 145, 2)),
                TypeMPathTableLocation = _bitter.ToInt32(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 149, 2)),
                OptionalTypeMPathTableLocation = _bitter.ToInt32(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 153, 2)),
                VolumeSetIdentifier = Encodings.CP437.GetString(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 191, 128)),
                PublisherIdentifier = Encodings.CP437.GetString(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 319, 128)),
                DataPreparerIdentifier = Encodings.CP437.GetString(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 447, 128)),
                ApplicationIdentifier = Encodings.CP437.GetString(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 575, 128)),
                CopyrightFileIdentifier = Encodings.CP437.GetString(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 703, 37)),
                AbstractFileIdentifier = Encodings.CP437.GetString(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 740, 37)),
                BibliographicFileIdentifier = Encodings.CP437.GetString(_slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 777, 37)),
                ApplicationData = _slicer.Slice(primaryVolumeDescriptor.Data, 0x17 + 884, 512)
            };
            
            
            
            throw new System.NotImplementedException();
        }
        
        
    }
}