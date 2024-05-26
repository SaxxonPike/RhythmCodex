using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters;

[Service]
public class NoteCommandStringDecoder : INoteCommandStringDecoder
{
    private static readonly char[] SkippedChars = [' ', '\t', '\n', '\r'];
    private static readonly char[] Delimiters = [';', ','];

    public List<Note> Decode(int columns, string notes)
    {
        return Do().ToList();

        IEnumerable<Note> Do()
        {
            var column = 0;
            var measures = notes.Split(Delimiters, StringSplitOptions.None);
            var measureId = 0;

            foreach (var measure in measures)
            {
                var buffer = new List<Note>();
                var rows = measure.Length / columns;
                var row = 0;

                foreach (var c in notes.Where(c => !SkippedChars.Contains(c)))
                {
                    if (c != '0')
                        yield return new Note
                        {
                            MetricOffset = new BigRational(measureId, row, rows),
                            Column = column,
                            Type = c
                        };

                    if (++column < columns)
                        continue;

                    column = 0;
                    row++;
                }

                measureId++;
            }
        }
    }
}