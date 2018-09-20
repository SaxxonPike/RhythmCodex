using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Converters;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    [Service]
    public class IsoSectorInfoDecoder : IIsoSectorInfoDecoder
    {
        private readonly IBcd _bcd;

        public IsoSectorInfoDecoder(IBcd bcd)
        {
            _bcd = bcd;
        }
        
        public IsoSectorInfo Decode(IsoSector sector)
        {
            var result = new IsoSectorInfo
            {
                Number = sector.Number,
                Data = sector.Data,
                UserDataOffset = 0, 
                UserDataLength = 2352
            };

            var data = sector.Data;

            if (!(data[0x0000] == 0x00 &&
                  data[0x0001] == 0xFF &&
                  data[0x0002] == 0xFF &&
                  data[0x0003] == 0xFF &&
                  data[0x0004] == 0xFF &&
                  data[0x0005] == 0xFF &&
                  data[0x0006] == 0xFF &&
                  data[0x0007] == 0xFF &&
                  data[0x0008] == 0xFF &&
                  data[0x0009] == 0xFF &&
                  data[0x000A] == 0xFF &&
                  data[0x000B] == 0x00))
            {
                return result;
            }

            result.Minutes = _bcd.FromBcd(data[0x000C]);
            result.Seconds = _bcd.FromBcd(data[0x000D]);
            result.Frames = _bcd.FromBcd(data[0x000E]);
            result.Mode = data[0x000F];

            switch (result.Mode)
            {
                case 0x01:
                    result.UserDataOffset = 16;
                    result.UserDataLength = 2048;
                    result.EdcOffset = 2064;
                    result.EccOffset = 2072;
                    break;
                case 0x02:
                    result.UserDataOffset = 24;
                    result.Id = data[0x0010];
                    result.Channel = data[0x0011];
                    result.EndOfRecord = (data[0x0012] & 0x01) != 0;
                    result.IsVideo = (data[0x0012] & 0x02) != 0;
                    result.IsAudio = (data[0x0012] & 0x04) != 0;
                    result.IsData = (data[0x0012] & 0x08) != 0;
                    result.Trigger = (data[0x0012] & 0x10) != 0;
                    result.Form = (data[0x0012] & 0x020) != 0 ? 2 : 1;
                    result.IsTimeDependent = (data[0x0012] & 0x40) != 0;
                    result.EndOfFile = (data[0x0012] & 0x80) != 0;
                    
                    if (result.IsAudio ?? false)
                    {
                        switch (data[0x0013] & 0x03)
                        {
                            case 0x0:
                                result.AudioChannels = 1;
                                break;
                            case 0x1:
                                result.AudioChannels = 2;
                                break;
                        }
                        switch (data[0x0013] & 0x0C)
                        {
                            case 0x0:
                                result.AudioRate = 37800;
                                break;
                            case 0x4:
                                result.AudioRate = 18900;
                                break;
                        }
                        switch (data[0x0013] & 0x30)
                        {
                            case 0x00:
                                result.AudioBitsPerSample = 4;
                                break;
                            case 0x10:
                                result.AudioBitsPerSample = 8;
                                break;
                        }

                        result.AudioEmphasis = (data[0x0013] & 0x40) != 0;
                    }

                    switch (result.Form ?? 1)
                    {
                        case 1:
                            result.UserDataLength = 2048;
                            result.EdcOffset = 2072;
                            result.EccOffset = 2076;
                            break;
                        case 2:
                            result.UserDataLength = (result.IsAudio ?? false) ? 2304 : 2324;
                            result.EdcOffset = 2348;
                            break;
                    }

                    break;
            }

            return result;
        }
    }
}