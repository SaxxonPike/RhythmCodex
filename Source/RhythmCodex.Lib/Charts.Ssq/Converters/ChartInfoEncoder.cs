using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Ssq.Converters
{
    [Service]
    public class ChartInfoEncoder : IChartInfoEncoder
    {
        public ChartInfo Suggest(IEnumerable<Event> events)
        {
            BigRational maxPlayer = -1;
            BigRational maxColumn = -1;

            foreach (var ev in events)
            {
                if (ev[NumericData.Player] != null)
                {
                    if (ev[NumericData.Player] > maxPlayer)
                        maxPlayer = ev[NumericData.Player].Value;
                }

                if (ev[NumericData.Column] != null)
                {
                    if (ev[NumericData.Column] > maxColumn)
                        maxColumn = ev[NumericData.Column].Value;
                }
            }

            return new ChartInfo
            {
                PanelCount = (int) maxColumn + 1,
                PlayerCount = (int) maxPlayer + 1,
                Type = GetGameType((int) maxColumn, (int) maxPlayer)
            };
        }

        private string GetGameType(int maxColumn, int maxPlayer)
        {
            return (maxPlayer + 1) switch
            {
                1 => (maxColumn + 1) switch
                {
                    1 => "dance-single",
                    2 => "dance-single",
                    3 => "dance-single",
                    4 => "dance-single",
                    5 => "dance-solo",
                    6 => "dance-solo",
                    7 => "dance-double",
                    8 => "dance-double",
                    _ => null
                },
                2 => (maxColumn + 1) switch
                {
                    1 => "dance-couple",
                    2 => "dance-couple",
                    3 => "dance-couple",
                    4 => "dance-couple",
                    5 => "dance-solo",
                    6 => "dance-solo",
                    7 => "dance-couple",
                    8 => "dance-couple",
                    _ => null
                },
                _ => null
            };
        }

        public int Encode(ChartInfo info)
        {
            throw new System.NotImplementedException();
        }
    }
}