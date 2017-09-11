using System.Collections.Generic;
using System.Linq;
using System.Text;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public class NoteCommandStringEncoder : INoteCommandStringEncoder
    {
        public string Encode(IEnumerable<Note> notes)
        {
            var resultBuilder = new StringBuilder();
            var isFirst = true;

            foreach (var measure in EncodeMeasures(notes))
            {
                var measureBuilder = new StringBuilder();
                foreach (var row in measure)
                    measureBuilder.AppendLine(new string(row));

                if (!isFirst)
                    resultBuilder.AppendLine(",");
                else
                    isFirst = false;

                resultBuilder.AppendLine(measureBuilder.ToString());
            }

            return resultBuilder.ToString();
        }

        private static IEnumerable<char[][]> EncodeMeasures(IEnumerable<Note> notes)
        {
            const int multiplier = 192;
            var half = new BigRational(1, multiplier * 2);

            var notesList = notes.AsList();
            var columns = notesList.Max(n => n.Column) + 1;
            var measures = notesList.GroupBy(n => n.MetricOffset.GetWholePart()).AsList();
            var maxMeasure = measures.Max(m => m.Key);

            for (var measureNumber = 0; measureNumber <= maxMeasure; measureNumber++)
            {
                var measure = measures.FirstOrDefault(m => m.Key == measureNumber) ?? Enumerable.Empty<Note>();
                var measureNotes = measure.ToArray();
                var grid = Enumerable.Range(0, multiplier).Select(i => Enumerable.Repeat(NoteType.None, columns).ToArray()).ToArray();

                foreach (var note in measureNotes)
                {
                    var row = (int)(note.MetricOffset.GetFractionPart() * multiplier + half);
                    if (row < 0)
                        row = 0;
                    if (row >= multiplier)
                        row = multiplier - 1;
                    grid[row][note.Column] = note.Type;
                }

                yield return grid;
            }
        }
    }
}
