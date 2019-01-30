using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Step2.Models;

namespace RhythmCodex.Step2.Mappers
{
    [Service]
    public class Step2EventMapper : IStep2EventMapper
    {
        public IEnumerable<int> Map(int panels)
        {
            if ((panels & 0x01) != 0)
                yield return 0;
            if ((panels & 0x02) != 0)
                yield return 3;
            if ((panels & 0x04) != 0)
                yield return 2;
            if ((panels & 0x08) != 0)
                yield return 1;                
        }

        public int Map(IEnumerable<int> panels)
        {
            var result = 0;
            foreach (var panel in panels)
            {
                switch (panel)
                {
                    case 0:
                        result |= 0x01;
                        break;
                    case 1:
                        result |= 0x08;
                        break;
                    case 2:
                        result |= 0x04;
                        break;
                    case 3:
                        result |= 0x08;
                        break;
                }
            }

            return result;
        }
    }
}