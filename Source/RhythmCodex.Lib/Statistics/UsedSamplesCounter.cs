using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Statistics
{
    [Service]
    public class UsedSamplesCounter : IUsedSamplesCounter
    {
        public ISet<int> GetUsedSamples(IEnumerable<IEvent> events)
        {
            var result = new HashSet<int>();
            var sounds = new Dictionary<(int, int, bool), int>();

            foreach (var ev in events.OrderBy(ev => ev[NumericData.LinearOffset] ?? ev[NumericData.MetricOffset]))
            {
                if (ev[NumericData.LoadSound] != null)
                {
                    var key = ((int) (ev[NumericData.Column] ?? -1), (int) (ev[NumericData.Player] ?? 0),
                        ev[FlagData.Scratch] == true);
                    sounds[key] = (int) ev[NumericData.LoadSound];
                }

                if (ev[NumericData.PlaySound] != null)
                {
                    var id = (int) ev[NumericData.PlaySound];
                    if (!result.Contains(id))
                        result.Add(id);
                }

                if (ev[FlagData.Note] == true)
                {
                    var key = ((int) (ev[NumericData.Column] ?? -1), (int) (ev[NumericData.Player] ?? 0),
                        ev[FlagData.Scratch] == true);
                    if (!sounds.ContainsKey(key)) 
                        continue;
                    var id = sounds[key];
                    if (!result.Contains(id))
                        result.Add(id);
                }
            }

            return result;
        }
    }
}