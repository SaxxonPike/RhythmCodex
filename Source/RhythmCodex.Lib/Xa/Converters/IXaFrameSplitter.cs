using System.Collections.Generic;

namespace RhythmCodex.Xa.Converters
{
    public interface IXaFrameSplitter
    {
        int GetStatus(byte[] frame, int channel);
        IEnumerable<int> GetData(byte[] frame, int channel);
    }
}