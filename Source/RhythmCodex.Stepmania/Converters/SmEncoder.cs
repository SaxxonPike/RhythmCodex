using System.Collections.Generic;
using System.Linq;
using Numerics;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public class SmEncoder
    {
        private readonly INoteEncoder _noteEncoder;
        private readonly INoteCommandStringEncoder _noteCommandStringEncoder;
        private readonly IGrooveRadarEncoder _grooveRadarEncoder;

        public SmEncoder(
            INoteEncoder noteEncoder,
            INoteCommandStringEncoder noteCommandStringEncoder,
            IGrooveRadarEncoder grooveRadarEncoder)
        {
            _noteEncoder = noteEncoder;
            _noteCommandStringEncoder = noteCommandStringEncoder;
            _grooveRadarEncoder = grooveRadarEncoder;
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
            
            var bpms = chartList
                .SelectMany(chart => chart.Events.Where(ev => ev[NumericData.Bpm] != null))
                .GroupBy(ev => ev[NumericData.MetricOffset])
                .Select(g => g.First());

            yield return new Command
            {
                Name = ChartTag.BpmsTag,
                Values = new[] { string.Join(",", bpms.Select(ev => $"{(decimal)(ev[NumericData.MetricOffset].Value * 4)}={(decimal)ev[NumericData.Bpm].Value}")) }
            };

            var stops = chartList
                .SelectMany(chart => chart.Events.Where(ev => ev[NumericData.Stop] != null))
                .GroupBy(ev => ev[NumericData.MetricOffset])
                .Select(g => g.First());

            yield return new Command
            {
                Name = ChartTag.StopsTag,
                Values = new[] { string.Join(",", stops.Select(ev => $"{(decimal)(ev[NumericData.MetricOffset].Value * 4)}={(decimal)ev[NumericData.Stop].Value}")) }
            };
        }

        public IEnumerable<Command> Encode(IMetadata metaData, IEnumerable<IChart> charts)
        {
            var chartList = charts.AsList();

            var metaCommands = TagsToEncode
                .Select(s => new Command
                {
                    Name = s,
                    Values = new[] {metaData[s] ?? string.Empty}
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
