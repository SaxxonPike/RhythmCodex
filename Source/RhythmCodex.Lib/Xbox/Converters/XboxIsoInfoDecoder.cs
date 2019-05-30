using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Xbox.Model;

namespace RhythmCodex.Xbox.Converters
{
    [Service]
    public class XboxIsoInfoDecoder : IXboxIsoInfoDecoder
    {
        public XboxIsoInfo Decode(byte[] sector)
        {
            return new XboxIsoInfo
            {
                DirectorySectorNumber = Bitter.ToInt32(sector, 0x14),
                DirectorySize = Bitter.ToInt32(sector, 0x18)
            };
        }
    }
}