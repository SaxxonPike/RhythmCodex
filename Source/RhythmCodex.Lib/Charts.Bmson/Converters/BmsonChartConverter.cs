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
    public BmsonFile Export(
        Chart chart,
        BmsonEncoderOptions options)
    {
        if (chart.Events.Any(ev => ev[NumericData.MetricOffset] == null))
            throw new RhythmCodexException("Metric offsets must all be populated in order to export to BMSON.");

        var orderedEvents = chart.Events
            .OrderBy(x => x[NumericData.MetricOffset])
            .ToList();

        var bpm = orderedEvents
            .Where(x => x[NumericData.Bpm] != null)
            .OrderBy(x => x[NumericData.MetricOffset])
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
        
        var sounds = new Dictionary<(int player, int column, bool scratch, bool footPedal), int>();
        var freezes = new Dictionary<(int player, int column, bool scratch, bool footPedal), BmsonNote>();
        var soundChannels = new Dictionary<int, BmsonSoundChannel>();
        var soundNames = new Dictionary<int, string>();

        foreach (var ev in orderedEvents)
        {
            var y = (long)Math.Round((double)(ev[NumericData.MetricOffset] * 4 * result.Info.Resolution)!);
            var playerId = (int)(ev[NumericData.Player] ?? -1);
            var columnId = (int)(ev[NumericData.Column] ?? -1);
            var scratch = (ev[FlagData.Scratch] ?? false) || (ev[FlagData.FreeZone] ?? false);
            var footPedal = ev[FlagData.FootPedal] ?? false;

            var lane = options.ChartType switch
            {
                BmsChartType.Beatmania => (playerId, columnId, scratch, footPedal) switch
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

            if (ev[FlagData.Measure] == true)
            {
                result.Lines.Add(new BmsonBarLine
                {
                    Y = y
                });
            }

            if (ev[FlagData.Note] != true)
            {
                var loadSoundId = (int)(ev[NumericData.LoadSound] ?? -1);
                if (loadSoundId >= 0)
                    sounds[(playerId, columnId, scratch, footPedal)] = loadSoundId;
            }

            var playSoundId = (int)(ev[NumericData.PlaySound] ?? -1);

            var soundFile = string.Empty;

            if (playSoundId >= 0 || ev[FlagData.Note] == true)
            {
                var freeze = lane != 0 && ev[FlagData.Freeze] == true;

                var targetSoundId = playSoundId >= 0
                    ? playSoundId
                    : sounds.GetValueOrDefault((playerId, columnId, scratch, footPedal), -1);

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

                if (lane != 0 && freezes.Remove((playerId, columnId, scratch, footPedal), out var freezeStartNote))
                    freezeStartNote.Length = y - freezeStartNote.Y;

                var note = new BmsonNote
                {
                    X = lane,
                    Y = y,
                    Length = 0,
                    Continuation = false
                };

                if (freeze)
                    freezes[(playerId, columnId, scratch, footPedal)] = note;

                soundChannel.Notes.Add(note);
            }
        }

        return result;
    }
}