using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public class IsoDescriptorSectorFinder : IIsoDescriptorSectorFinder
    {
        private static readonly byte[] StandardIdentifier = {0x43, 0x44, 0x30, 0x30, 0x31};

        public IEnumerable<IsoSectorInfo> Find(IEnumerable<IsoSectorInfo> sectors)
        {
            var mode1Sectors = sectors.Where(s => s.Mode == 1).ToList();
            var currentMinute = 0;
            var currentSecond = 2;
            var currentFrame = 16;

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

                yield return descriptorSector;
                
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
        }
    }

    public interface IIsoDescriptorSectorFinder
    {
        IEnumerable<IsoSectorInfo> Find(IEnumerable<IsoSectorInfo> sectors);
    }
}