using System.Collections.Generic;

namespace RhythmCodex.Xa.Converters
{
    public class XaFrameSplitter : IXaFrameSplitter
    {
        public int GetStatus(byte[] frame, int channel) => 
            frame[(channel & 7) + 4];

        public IEnumerable<int> Get4BitData(byte[] frame, int channel)
        {
            var channelOffset = 0x10 + ((channel & 7) >> 1);
            var dataShift = (channel & 1) << 2;

            for (var i = 0; i < 28; i++)
                yield return (frame[channelOffset + (i << 2)] >> dataShift) & 0xF;
        }
        
        public IEnumerable<int> Get8BitData(byte[] frame, int channel)
        {
            var channelOffset = 0x10 + (channel & 3);

            for (var i = 0; i < 28; i++)
                yield return frame[channelOffset + (i << 2)];
        }
    }
}