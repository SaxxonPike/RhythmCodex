using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Riff.Processing
{
    [Service]
    public class SoundConsolidator : ISoundConsolidator
    {
        private struct PlayedEvent
        {
            public int Index { get; set; }
            public BigRational Panning { get; set; }
            public BigRational Offset { get; set; }
        }

        private struct MatchedSound
        {
            public int A { get; set; }
            public int B { get; set; }
        }
        
        public void Consolidate(IEnumerable<ISound> sounds, IEnumerable<Event> events)
        {
            // Evaluate if two samples should be combined based on panning and play time.
            bool Compare(IList<PlayedEvent> a, IList<PlayedEvent> b)
            {
                if (a.Count != b.Count)
                    return false;
                
                for (var i = 0; i < a.Count; i++)
                {
                    if (a[i].Offset != b[i].Offset ||
                        a[i].Panning != 1 - b[i].Panning)
                        return false;
                }

                return true;
            }
            
            var soundList = sounds.AsList();
            var eventList = events.AsList();
            var matches = new List<MatchedSound>();

            // Loaded samples are excluded because they can be played by the player.
            var loaded = eventList
                .Where(e => e?[NumericData.LoadSound] != null)
                .Select(e => e[NumericData.LoadSound])
                .Distinct()
                .ToList();

            // Get all the times a sample is played.
            var played = eventList
                .Where(e => e?[NumericData.PlaySound] != null && !loaded.Contains(e[NumericData.PlaySound]))
                .Select(e =>
                {
                    var index = (int) e[NumericData.PlaySound];
                    return new PlayedEvent
                    {
                        Index = index,
                        Offset = e[NumericData.LinearOffset] ?? 0,
                        Panning = e[NumericData.Panning] ?? 
                                  soundList.FirstOrDefault(s => index == (int)s[NumericData.Id].Value)[NumericData.Panning] ?? 
                                  new BigRational(1, 2)
                    };
                })
                .ToList();

            // Group each sample's played times.
            var groups = played
                .GroupBy(p => p.Index)
                .ToDictionary(g => g.Key, g => g.ToList())
                .ToList();

            // Compare every sample's played times to every other sample's played times.
            for (var i = 0; i < groups.Count; i++)
            {
                for (var j = i + 1; j < groups.Count; j++)
                {
                    if (Compare(groups[i].Value, groups[j].Value))
                        matches.Add(new MatchedSound
                        {
                            A = groups[i].Key,
                            B = groups[j].Key
                        });
                }
            }
            
            // Consolidate each match.
            var doneMatch = new List<int>();
            foreach (var match in matches)
            {
                if (doneMatch.Contains(match.A) || doneMatch.Contains(match.B))
                    continue;
                
                var soundA = soundList.FirstOrDefault(s => s[NumericData.Id] == match.A);
                var soundB = soundList.FirstOrDefault(s => s[NumericData.Id] == match.B);

                if (soundA == null || soundB == null)
                    continue;

                doneMatch.Add(match.A);
                doneMatch.Add(match.B);
                
                foreach (var sample in soundB.Samples.ToList())
                    soundA.Samples.Add(sample);
                soundB.Samples.Clear();

                soundA[NumericData.Panning] = soundB[NumericData.Panning] = new BigRational(1, 2);
            }
            
        }
    }
}