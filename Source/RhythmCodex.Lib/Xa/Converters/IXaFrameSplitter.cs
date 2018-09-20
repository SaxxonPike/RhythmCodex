using System.Collections.Generic;

namespace RhythmCodex.Xa.Converters
{
    public interface IXaFrameSplitter
    {
        int GetStatus(byte[] frame, int channel);
        IEnumerable<int> Get4BitData(byte[] frame, int channel);
        IEnumerable<int> Get8BitData(byte[] frame, int channel);
    }
}