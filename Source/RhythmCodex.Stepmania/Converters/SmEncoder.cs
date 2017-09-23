using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    [Service]
    public class SmEncoder : ISmEncoder
    {
        private readonly INoteEncoder _noteEncoder;
        private readonly INoteCommandStringEncoder _noteCommandStringEncoder;
        private readonly IGrooveRadarEncoder _grooveRadarEncoder;
        private readonly INumberFormatter _numberFormatter;

        public SmEncoder(
            INoteEncoder noteEncoder,
            INoteCommandStringEncoder noteCommandStringEncoder,
            IGrooveRadarEncoder grooveRadarEncoder,
            INumberFormatter numberFormatter)
        {
            _noteEncoder = noteEncoder;
            _noteCommandStringEncoder = noteCommandStringEncoder;
            _grooveRadarEncoder = grooveRadarEncoder;
            _numberFormatter = numberFormatter;
        }

        private static readonly IEnumerable<string> TagsToEncode = new[]
        {
            ChartTag.TitleTag,
            ChartTag.ArtistTag,
            ChartTag.SubTitleTag,
            ChartTag.GenreTag,
            ChartTag.TitleTranslitTag,
            ChartTag.ArtistTranslitTag,
            ChartTag.SubTitleTranslitTag,
            ChartTag.CreditTag,
            ChartTag.BannerTag,
            ChartTag.BackgroundTag,
            ChartTag.LyricsPathTag,
            ChartTag.CdTitleTag,
            ChartTag.MusicTag,
            ChartTag.OffsetTag,
            ChartTag.SampleStartTag,
            ChartTag.SampleLengthTag,
            ChartTag.DisplayBpmTag,
            ChartTag.BgChangesTag,
            ChartTag.FgChangesTag
        };

        private IEnumerable<Command> GetTimingCommands(IEnumerable<IChart> charts)
        {
            var chartList = charts.AsList();
            var places = StepmaniaConstants.DecimalPlaces;

            var bpms = chartList
                .SelectMany(chart => chart.Events.Where(ev => ev[NumericData.Bpm] != null))
                .GroupBy(ev => ev[NumericData.MetricOffset])
                .Select(g => g.First());

            yield return new Command
            {
                Name = ChartTag.BpmsTag,
                Values = new[]
                {
                    string.Join(",",
                        bpms.Select(ev =>
                        {
                            var key = _numberFormatter.Format(ev[NumericData.MetricOffset].Value * 4, places);
                            var value = _numberFormatter.Format(ev[NumericData.Bpm].Value, places);
                            return $"{key}={value}";
                        }))
                }
            };

            var stops = chartList
                .SelectMany(chart => chart.Events.Where(ev => ev[NumericData.Stop] != null))
                .GroupBy(ev => ev[NumericData.MetricOffset])
                .Select(g => g.First());

            yield return new Command
            {
                Name = ChartTag.StopsTag,
                Values = new[]
                {
                    string.Join(",",
                        stops.Select(ev =>
                        {
                            var key = _numberFormatter.Format(ev[NumericData.MetricOffset].Value * 4, places);
                            var value = _numberFormatter.Format(ev[NumericData.Stop].Value, places);
                            return $"{key}={value}";
                        }))
                }
            };
        }

        public IEnumerable<Command> Encode(ChartSet chartSet)
        {
            var chartList = chartSet.Charts.AsList();

            var metaCommands = TagsToEncode
                .Select(s => new Command
                {
                    Name = s,
                    Values = new[] {chartSet.Metadata[s] ?? string.Empty}
                });

            var timingCommands = GetTimingCommands(chartList);

            var noteCommands = chartList.Select(chart => new Command
            {
                Name = ChartTag.NotesTag,
                Values = new[]
                {
                    chart[NotesCommandTag.TypeTag] ?? string.Empty,
                    chart[NotesCommandTag.DescriptionTag] ?? string.Empty,
                    chart[NotesCommandTag.DifficultyTag] ?? string.Empty,
                    $"{(chart[NumericData.PlayLevel] ?? BigRational.One).GetWholePart()}",
                    _grooveRadarEncoder.Encode(chart),
                    _noteCommandStringEncoder.Encode(_noteEncoder.Encode(chart.Events))
                }
            });

            return metaCommands
                .Concat(timingCommands)
                .Concat(noteCommands);
        }
    }
}