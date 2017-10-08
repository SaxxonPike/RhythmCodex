using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    [Service]
    public class NoteCommandStringEncoder : INoteCommandStringEncoder
    {
        private static readonly BigInteger MinimumQuantization = 4;
        private static readonly BigInteger MaximumQuantization = 192;

        private readonly IQuantizer _quantizer;

        public NoteCommandStringEncoder(IQuantizer quantizer)
        {
            _quantizer = quantizer;
        }

        public string Encode(IEnumerable<Note> notes)
        {
            var resultBuilder = new StringBuilder();
            var isFirst = true;

            foreach (var measure in EncodeMeasures(notes))
            {
                var measureBuilder = new StringBuilder();
                measureBuilder.AppendLine();

                foreach (var row in measure)
                    measureBuilder.AppendLine(new string(row));

                if (!isFirst)
                    resultBuilder.Append(",");
                else
                    isFirst = false;

                resultBuilder.Append(measureBuilder);
            }

            return resultBuilder.ToString();
        }

        private IEnumerable<char[][]> EncodeMeasures(IEnumerable<Note> notes)
        {
            var notesList = notes.AsList();
            var columns = notesList.Max(n => n.Column) + 1;
            var measures = notesList.GroupBy(n => n.MetricOffset.GetWholePart()).AsList();
            var maxMeasure = measures.Max(m => m.Key);

            for (var measureNumber = 0; measureNumber <= maxMeasure; measureNumber++)
            {
                var measure = measures.FirstOrDefault(m => m.Key == measureNumber) ?? Enumerable.Empty<Note>();
                var measureNotes = measure.ToArray();
                var quantization = _quantizer.GetQuantization(measureNotes.Select(n => n.MetricOffset),
                    MinimumQuantization, MaximumQuantization);
                var half = new BigRational(1, quantization * 2);

                var grid = Enumerable.Range(0, quantization)
                    .Select(i => Enumerable.Repeat(NoteType.None, columns).ToArray()).ToArray();

                foreach (var note in measureNotes)
                {
                    var row = (int) (note.MetricOffset.GetFractionPart() * quantization + half);
                    if (row < 0)
                        row = 0;
                    if (row >= quantization)
                        row = quantization - 1;
                    grid[row][note.Column] = note.Type;
                }

                yield return grid;
            }
        }
    }
}