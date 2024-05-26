using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;

namespace RhythmCodex.Ssq.Converters;

[Service]
public class StepPanelSplitter : IStepPanelSplitter
{
    public List<int> Split(int panels)
    {
        return Do().ToList();

        IEnumerable<int> Do()
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