using System.Collections.Generic;

namespace RhythmCodex.Xa.Converters
{
    public class XaFrameSplitter : IXaFrameSplitter
    {
        public int GetStatus(byte[] frame, int channel) => 
            frame[(channel & 3) | ((channel & 4) << 1)];

        public IEnumerable<int> GetData(byte[] frame, int channel)
        {
            var channelOffset = 0x10 + (channel >> 1);
            var dataShift = (channel & 1) << 2;

            for (var i = 0; i < 28; i++)
                yield return (frame[channelOffset + (i << 2)] >> dataShift) & 0xF;
        }
    }
}