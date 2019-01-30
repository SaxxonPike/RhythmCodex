using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class StepPanelSplitter : IStepPanelSplitter
    {
        public IEnumerable<int> Split(int panels)
        {
            var n = 0;

            if (panels == -1)
            {
                yield return 0;
                panels ^= int.MinValue;
                n++;
            }

            while (panels > 0)
            {
                if ((panels & 1) != 0)
                    yield return n;
                panels >>= 1;
                n++;
            }
        }
    }
}