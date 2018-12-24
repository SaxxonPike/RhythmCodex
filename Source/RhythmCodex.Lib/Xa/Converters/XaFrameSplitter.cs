using System;
using System.Collections.Generic;

namespace RhythmCodex.Xa.Converters
{
    public class XaFrameSplitter : IXaFrameSplitter
    {
        public int GetStatus(ReadOnlyMemory<byte> frame, int channel) => 
            frame.Span[(channel & 7) + 4];

        public IEnumerable<int> Get4BitData(ReadOnlyMemory<byte> frame, int channel)
        {
            var channelOffset = 0x10 + ((channel & 7) >> 1);
            var dataShift = (channel & 1) << 2;

            for (var i = 0; i < 28; i++)
                yield return (frame.Span[channelOffset + (i << 2)] >> dataShift) & 0xF;
        }
        
        public IEnumerable<int> Get8BitData(ReadOnlyMemory<byte> frame, int channel)
        {
            var channelOffset = 0x10 + (channel & 3);

            for (var i = 0; i < 28; i++)
                yield return frame.Span[channelOffset + (i << 2)];
        }
    }
}