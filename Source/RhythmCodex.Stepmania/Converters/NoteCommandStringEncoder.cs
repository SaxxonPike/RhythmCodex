using System.Collections.Generic;
using System.Linq;
using Numerics;
using RhythmCodex.Extensions;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public class NoteCommandStringEncoder : INoteCommandStringEncoder
    {
        public string Encode(IEnumerable<Note> notes)
        {
            var measures = EncodeMeasures(notes).ToArray();
            
        }

        private static IEnumerable<char[][]> EncodeMeasures(IEnumerable<Note> notes)
        {
            var notesList = notes.AsList();
            var columns = notesList.Max(n => n.Column);
            var measures = notesList.GroupBy(n => n.MetricOffset.GetWholePart()).AsList();
            var maxMeasure = measures.Max(m => m.Key);

            for (var measureNumber = 0; measureNumber <= maxMeasure; measureNumber++)
            {
                var measure = measures.FirstOrDefault(m => m.Key == measureNumber) ?? Enumerable.Empty<Note>();
                var measureNotes = measure.ToArray();
                var multiplier = measureNotes.Select(m => m.MetricOffset).GetWholeMultiplier();
                if (multiplier < 4)
                    multiplier = 4;

                var grid = Enumerable.Range(0, (int)multiplier).Select(i => Enumerable.Repeat(NoteType.None, columns).ToArray()).ToArray();

                foreach (var note in measureNotes)
                {
                    var row = (int)(note.MetricOffset.GetFractionPart() * multiplier);
                    grid[row][note.Column] = note.Type;
                }

                yield return grid;
            }
        }
    }
}
