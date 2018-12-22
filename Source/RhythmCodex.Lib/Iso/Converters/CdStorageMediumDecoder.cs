using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Converters;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    [Service]
    public class CdStorageMediumDecoder : ICdStorageMediumDecoder
    {
        private static readonly byte[] StandardIdentifier = {0x43, 0x44, 0x30, 0x30, 0x31};
        
        private readonly ISlicer _slicer;
        private readonly IBitter _bitter;
        private readonly IBcd _bcd;

        public CdStorageMediumDecoder(ISlicer slicer, IBitter bitter, IBcd bcd)
        {
            _slicer = slicer;
            _bitter = bitter;
            _bcd = bcd;
        }
        
        public Iso9660StorageMedium Decode(IEnumerable<Iso9660SectorInfo> sectors)
        {
            var result = new Iso9660StorageMedium
            {
                BootRecords = new List<Iso9660BootRecord>(),
                Volumes = new List<Iso9660Volume>()
            };
            
            var mode1Sectors = sectors.Where(s => s.Mode == 1).ToList();
            var currentMinute = 0;
            var currentSecond = 2;
            var currentFrame = 16;
            var descriptorSectors = new List<Iso9660SectorInfo>();

            while (true)
            {
                var descriptorSector = mode1Sectors.SingleOrDefault(s => s.Frames == currentFrame &&
                                                                         s.Seconds == currentSecond &&
                                                                         s.Minutes == currentMinute);
                
                if (descriptorSector == null)
                    throw new RhythmCodexException($"Volume descriptors incomplete. MSF={currentMinute:000}:{currentSecond:00}:{currentFrame:00}");

                var userData = descriptorSector.UserData;
                
                if (!userData.Slice(0x0001, 5).SequenceEqual(StandardIdentifier))
                    throw new RhythmCodexException($"Standard identifier is incorrect. MSF={currentMinute:000}:{currentSecond:00}:{currentFrame:00}");

                descriptorSectors.Add(descriptorSector);
                
                if (userData[0x0000] == 255)
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

            var bootDescriptors = descriptorSectors.Where(s => s.UserData[0] == 0x00).ToList();
            var primaryVolumeDescriptor = descriptorSectors.Single(s => s.UserData[0] == 0x01);
            var supplementaryVolumeDescriptors = descriptorSectors.Where(s => s.UserData[0] == 0x02).ToList();
            var partitionDescriptors = descriptorSectors.Where(s => s.UserData[0] == 0x03).ToList();

            foreach (var bootDescriptor in bootDescriptors)
            {
                result.BootRecords.Add(new Iso9660BootRecord
                {
                    BootSystemIdentifier = Encodings.CP437.GetString(bootDescriptor.UserData.Slice(7, 32)),
                    BootIdentifier = Encodings.CP437.GetString(bootDescriptor.UserData.Slice(39, 32)),
                    BootSystemData = bootDescriptor.UserData.Slice(71, 1977).ToArray()
                });
            }

            var primaryVolume = new Iso9660Volume
            {
                SystemIdentifier = Encodings.CP437.GetString(primaryVolumeDescriptor.UserData.Slice(8, 32)),
                VolumeIdentifier = Encodings.CP437.GetString(primaryVolumeDescriptor.UserData.Slice(40, 32)),
                SpaceSize = _bitter.ToInt32(primaryVolumeDescriptor.UserData.Slice(80, 8)),
                SetSize = _bitter.ToInt32(primaryVolumeDescriptor.UserData.Slice(120, 4)),
                SequenceNumber = _bitter.ToInt32(primaryVolumeDescriptor.UserData.Slice(124, 4)),
                LogicalBlockSize = _bitter.ToInt32(primaryVolumeDescriptor.UserData.Slice(128, 4)),
                PathTableSize = _bitter.ToInt32(primaryVolumeDescriptor.UserData.Slice(132, 8)),
                TypeLPathTableLocation = _bitter.ToInt32(primaryVolumeDescriptor.UserData.Slice(140, 4)),
                OptionalTypeLPathTableLocation = _bitter.ToInt32(primaryVolumeDescriptor.UserData.Slice(144, 4)),
                TypeMPathTableLocation = _bitter.ToInt32(primaryVolumeDescriptor.UserData.Slice(148, 4)),
                OptionalTypeMPathTableLocation = _bitter.ToInt32(primaryVolumeDescriptor.UserData.Slice(152, 4)),
                // directory record at 156 for 34 bytes
                VolumeSetIdentifier = Encodings.CP437.GetString(primaryVolumeDescriptor.UserData.Slice(190, 128)),
                PublisherIdentifier = Encodings.CP437.GetString(primaryVolumeDescriptor.UserData.Slice(318, 128)),
                DataPreparerIdentifier = Encodings.CP437.GetString(primaryVolumeDescriptor.UserData.Slice(446, 128)),
                ApplicationIdentifier = Encodings.CP437.GetString(primaryVolumeDescriptor.UserData.Slice(574, 128)),
                CopyrightFileIdentifier = Encodings.CP437.GetString(primaryVolumeDescriptor.UserData.Slice(702, 38)),
                AbstractFileIdentifier = Encodings.CP437.GetString(primaryVolumeDescriptor.UserData.Slice(740, 36)),
                BibliographicFileIdentifier = Encodings.CP437.GetString(primaryVolumeDescriptor.UserData.Slice(776, 37)),
                ApplicationData = primaryVolumeDescriptor.UserData.Slice(883, 512).ToArray()
            };
            
            result.Volumes.Add(primaryVolume);

            return result;
        }
        
        
    }
}