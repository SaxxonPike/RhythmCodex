using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters;

[Service]
public class NoteEncoder : INoteEncoder
{
    private readonly ILogger _logger;

    public NoteEncoder(ILogger logger)
    {
        _logger = logger;
    }

    public IList<Note> Encode(IEnumerable<IEvent> events)
    {
        IEnumerable<Note> Do()
        {
            var result = new List<Note>();
            var eventList = events.AsList();
            var columnCount = (int) eventList.Max(e => e[NumericData.Column] ?? BigRational.Zero) + 1;
            var playerCount = (int) eventList.Max(e => e[NumericData.Player] ?? BigRational.Zero) + 1;

            foreach (var ev in eventList.Where(e => e[NumericData.MetricOffset] != null))
            {
                var offset = ev[NumericData.MetricOffset];
                if (offset == null)
                    throw new RhythmCodexException($"{nameof(NumericData.MetricOffset)} must be present.");

                var offsetValue = offset.Value;

                if (ev[NumericData.Column] != null)
                {
                    var column = (int) ev[NumericData.Column].Value +
                                 (int) (ev[NumericData.Player] ?? BigRational.Zero) * columnCount;

                    if (ev[FlagData.Note] == true)
                    {
                        result.Add(new Note
                        {
                            MetricOffset = offsetValue,
                            Type = NoteType.Step,
                            Column = column
                        });
                        continue;
                    }

                    if (ev[FlagData.Freeze] == true)
                    {
                        result.Add(new Note
                        {
                            MetricOffset = offsetValue,
                            Type = NoteType.Tail,
                            Column = column
                        });
                        continue;
                    }
                }

                if (ev[FlagData.Shock] == true)
                    result.AddRange(Enumerable.Range(0, columnCount * playerCount).Select(i => new Note
                    {
                        MetricOffset = offsetValue,
                        Type = NoteType.Mine,
                        Column = i
                    }));
            }

            ApplyFreezeHeads(result);
            return result;
        }

        return Do().ToList();
    }

    private void ApplyFreezeHeads(IEnumerable<Note> notes)
    {
        var freezeColumns = new HashSet<int>();
        foreach (var note in notes.Reverse())
            if (note.Type == NoteType.Tail)
            {
                freezeColumns.Add(note.Column);
            }
            else if (note.Type == NoteType.Step && freezeColumns.Contains(note.Column))
            {
                freezeColumns.Remove(note.Column);
                note.Type = NoteType.Freeze;
            }

        foreach (var column in freezeColumns)
            _logger.Warning($"Column {column} has a freeze tail but no suitable freeze head");
    }
}