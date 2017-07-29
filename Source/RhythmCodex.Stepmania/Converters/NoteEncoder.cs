using System.Collections.Generic;
using System.Linq;
using Numerics;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public class NoteEncoder : INoteEncoder
    {
        public IEnumerable<Note> Encode(IEnumerable<IEvent> events)
        {
            var result = new List<Note>();
            var eventList = events.AsList();
            var columnCount = (int)eventList.Max(e => e[NumericData.Column] ?? BigRational.Zero) + 1;

            foreach (var ev in eventList.Where(e => e[NumericData.MetricOffset] != null))
            {
                var offset = ev[NumericData.MetricOffset].Value;
                
                if (ev[NumericData.Column] != null)
                {
                    var column = (int)ev[NumericData.Column].Value + (int)(ev[NumericData.Player] ?? BigRational.Zero) * columnCount;
                    
                    if (ev[FlagData.Note] == true)
                    {
                        result.Add(new Note
                        {
                            MetricOffset = offset,
                            Type = NoteType.Step,
                            Column = column
                        });
                        continue;
                    }

                    if (ev[FlagData.Freeze] == true)
                    {
                        result.Add(new Note
                        {
                            MetricOffset = offset,
                            Type = NoteType.Tail,
                            Column = column
                        });
                        continue;
                    }
                }

                if (ev[FlagData.Shock] == true)
                {
                    result.AddRange(Enumerable.Range(0, columnCount).Select(i => new Note
                    {
                        MetricOffset = offset,
                        Type = NoteType.Mine,
                        Column = i
                    }));
                }
            }
            
            return result;
        }
    }
}
