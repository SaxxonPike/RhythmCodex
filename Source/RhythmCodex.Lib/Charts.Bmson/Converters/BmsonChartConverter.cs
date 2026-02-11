using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Bms.Converters;
using RhythmCodex.Charts.Bmson.Model;
using RhythmCodex.Charts.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Bmson.Converters;

[Service]
public class BmsonChartConverter : IBmsonChartConverter
{
    private record struct LaneId(int PlayerId, int ColumnId, bool Scratch, bool FootPedal);
    
    public BmsonFile Export(
        Chart chart,
        BmsonEncoderOptions options)
    {
        if (chart.Events.Any(ev => ev[NumericData.MetricOffset] == null))
            throw new RhythmCodexException("Metric offsets must all be populated in order to export to BMSON.");

        var orderedEvents = chart.Events
            .OrderBy(x => x[NumericData.MetricOffset] ?? 0)
            .ThenBy(x => x[NumericData.SourceIndex] ?? 0)
            .ToList();

        var bpm = orderedEvents
            .Where(x => x[NumericData.Bpm] != null)
            .Select(x => x[NumericData.Bpm])
            .FirstOrDefault() ?? chart[NumericData.Bpm] ?? 120;

        var result = new BmsonFile
        {
            Version = "1.0.0",
            Info =
            {
                Title = chart[StringData.Title],
                SubTitle = chart[StringData.Subtitle],
                Artist = chart[StringData.Artist],
                SubArtists = chart[StringData.Subartist] is { } subArtist ? [subArtist] : null,
                Genre = chart[StringData.Genre],
                ModeHint = options.ModeHint ?? chart[StringData.Type],
                ChartName = chart[StringData.Description],
                Level = (long?)chart[NumericData.PlayLevel] ?? 0,
                InitBpm = (double)bpm,
                Resolution = 420
            }
        };

        var rawSounds = new Dictionary<LaneId, int>();
        var sounds = new Dictionary<(int player, int column), int>();
        var freezes = new Dictionary<(int player, int column), BmsonNote>();
        var soundChannels = new Dictionary<int, BmsonSoundChannel>();
        var soundNames = new Dictionary<int, string>();
        var measureLines = new HashSet<long>();

        foreach (var ev in orderedEvents)
        {
            var y = (long)Math.Round((double)(ev[NumericData.MetricOffset] * 4 * result.Info.Resolution)!);
            var playerId = (int)(ev[NumericData.Player] ?? -1);
            var columnId = (int)(ev[NumericData.Column] ?? -1);
            var scratch = (ev[FlagData.Scratch] ?? false) || (ev[FlagData.FreeZone] ?? false);
            var footPedal = ev[FlagData.FootPedal] ?? false;
            var laneId = new LaneId(playerId, columnId, scratch, footPedal);

            var lane = options.ChartType switch
            {
                BmsChartType.Beatmania => laneId switch
                {
                    // Invalid player
                    (> 1 or < 0, _, _, _) => 0,
                    // Invalid key
                    (_, < 0 or > 7, false, false) => 0,
                    // 2P scratch
                    (1, _, true, _) => 16,
                    // 2P footpedal
                    (1, _, _, true) => 14,
                    // 2P key
                    (1, _, _, _) => columnId + 9,
                    // 1P scratch
                    (0, _, true, _) => 8,
                    // 1P footpedal
                    (0, _, _, true) => 6,
                    // 1P key
                    (0, _, _, _) => columnId + 1
                },
                BmsChartType.Popn => columnId switch
                {
                    < 0 or > 8 => 0,
                    _ => columnId + 1
                },
                _ => 0
            };

            if (ev[NumericData.Bpm] != null)
            {
                var bpmValue = (double)ev[NumericData.Bpm]!;

                if (bpmValue > 0)
                {
                    result.BpmEvents.Add(new BmsonBpmEvent
                    {
                        Y = y,
                        Bpm = bpmValue
                    });
                }
            }

            if (ev[FlagData.Measure] == true && measureLines.Add(y))
            {
                result.Lines.Add(new BmsonBarLine
                {
                    Y = y
                });
            }

            if (ev[NumericData.LoadSound] != null)
            {
                rawSounds[laneId] = (int)ev[NumericData.LoadSound]!;
                sounds[(playerId, lane)] = (int)ev[NumericData.LoadSound]!;
            }

            var soundFile = string.Empty;

            if (ev[FlagData.Note] == true || ev[NumericData.PlaySound] != null)
            {
                var freeze = lane != 0 && ev[FlagData.Freeze] == true;

                var targetSoundId = ev[NumericData.PlaySound] != null
                    ? (int)ev[NumericData.PlaySound]!
                    : lane > 0
                        ? sounds.GetValueOrDefault((playerId, lane), -1)
                        : rawSounds.GetValueOrDefault(laneId, -1);

                if (targetSoundId >= 0)
                {
                    if (!soundNames.TryGetValue(targetSoundId, out soundFile))
                    {
                        soundFile = options.WavNameTransformer?.Invoke(targetSoundId) ?? "";
                        soundNames[targetSoundId] = soundFile;
                    }
                }

                if (!soundChannels.TryGetValue(targetSoundId, out var soundChannel))
                {
                    soundChannel = new BmsonSoundChannel
                    {
                        Name = soundFile
                    };

                    soundChannels.Add(targetSoundId, soundChannel);
                    result.SoundChannels.Add(soundChannel);
                }

                if (lane != 0 && freezes.Remove((playerId, lane), out var freezeStartNote))
                    freezeStartNote.Length = y - freezeStartNote.Y;

                var note = new BmsonNote
                {
                    X = lane,
                    Y = y,
                    Length = 0,
                    Continuation = false
                };

                if (freeze)
                    freezes[(playerId, lane)] = note;

                if (lane > 0)
                {
                    sounds[(playerId, lane)] = targetSoundId;
                    rawSounds[laneId] = targetSoundId;
                }

                soundChannel.Notes.Add(note);
            }
        }

        return result;
    }
}