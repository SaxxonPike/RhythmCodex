using System.Collections.Generic;

namespace RhythmCodex.Audio
{
    public interface ISample
    {
        IEnumerable<float> Data { get; set; }
    }
}