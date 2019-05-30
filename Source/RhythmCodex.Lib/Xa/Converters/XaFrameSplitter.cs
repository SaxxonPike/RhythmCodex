using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Xa.Converters
{
    [Service]
    public class XaFrameSplitter : IXaFrameSplitter
    {
        public int GetStatus(ReadOnlySpan<byte> frame, int channel) => 
            frame[(channel & 7) + 4];

        public void Get4BitData(ReadOnlySpan<byte> frame, Span<int> buffer, int channel)
        {
            if (buffer == null)
                throw new RhythmCodexException("Buffer cannot be null.");
            
            if (buffer.Length != 28)
                throw new RhythmCodexException("Buffer must have a length of 28.");
            
            var channelOffset = 0x10 + ((channel & 7) >> 1);
            var dataShift = (channel & 1) << 2;

            for (var i = 0; i < 28; i++)
                buffer[i] = (frame[channelOffset + (i << 2)] >> dataShift) & 0xF;
        }
        
        public void Get8BitData(ReadOnlySpan<byte> frame, Span<int> buffer, int channel)
        {
            if (buffer == null)
                throw new RhythmCodexException("Buffer cannot be null.");
            
            if (buffer.Length != 28)
                throw new RhythmCodexException("Buffer must have a length of 28.");
            
            var channelOffset = 0x10 + (channel & 3);

            for (var i = 0; i < 28; i++)
                buffer[i] = frame[channelOffset + (i << 2)];
        }
    }
}