using System;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Charts.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Archs.Djmain.Converters;

[Service]
public class DjmainEventMetadataDecoder : IDjmainEventMetadataDecoder
{
    private void SetCommon(Event ev, DjmainEventType command, int param0, int param1)
    {
        switch (command)
        {
            case DjmainEventType.Bpm:
            {
                ev[NumericData.Bpm] = param1 | (param0 << 8);
                break;
            }

            case DjmainEventType.End:
            {
                ev[FlagData.End] = true;
                break;
            }

            case DjmainEventType.Bgm:
            {
                ev[NumericData.Panning] = 1 - new BigRational(Math.Max(param0 - 1, 0), 14);
                ev[NumericData.PlaySound] = param1 - 1;
                break;
            }

            case DjmainEventType.JudgeTiming:
            {
                ev[NumericData.JudgeNumber] = param0;
                ev[NumericData.JudgeTiming] = param1;
                ev[NumericData.SourceColumn] = param0;
                break;
            }

            case DjmainEventType.JudgeSound:
            {
                ev[NumericData.JudgeNumber] = param0 & 0x7;
                ev[NumericData.Player] = param0 >> 3;
                ev[NumericData.SourceColumn] = param0;
                ev[NumericData.JudgeSound] = param1 - 1;
                break;
            }

            case DjmainEventType.JudgeTrigger:
            {
                ev[NumericData.Player] = param0 >> 3;
                ev[NumericData.SourceColumn] = param0;
                ev[NumericData.Trigger] = param1;
                break;
            }

            case DjmainEventType.PhraseSelect:
            {
                ev[NumericData.SourceColumn] = param0;
                ev[NumericData.Phrase] = param1;
                break;
            }
        }
    }

    public void AddBeatmaniaMetadata(Event ev, DjmainChartEvent ce)
    {
        var command = (DjmainEventType) (ce.Param0 & 0xF);
        var param0 = ce.Param0 >> 4;
        var param1 = ce.Param1;

        ev[NumericData.SourceCommand] = ce.Param0;
        ev[NumericData.SourceData] = ce.Param1;

        switch (command)
        {
            case DjmainEventType.Marker:
            case DjmainEventType.SoundSelect:
                switch ((DjmainBeatmaniaColumnType) param0)
                {
                    case DjmainBeatmaniaColumnType.Player0Scratch:
                    case DjmainBeatmaniaColumnType.Player1Scratch:
                    {
                        ev[FlagData.Scratch] = true;
                        ev[NumericData.Player] = param0 & 1;
                        break;
                    }

                    case DjmainBeatmaniaColumnType.Player0Measure:
                    case DjmainBeatmaniaColumnType.Player1Measure:
                    {
                        // Sometimes you get commands like 0xD1 (2p measure sound select?? wtf??)
                        // which are absolutely absurd, so we don't bother.
                        if (command == DjmainEventType.Marker)
                            ev[FlagData.Measure] = true;

                        ev[NumericData.Player] = param0 & 1;
                        break;
                    }

                    case DjmainBeatmaniaColumnType.Player0FreeScratch:
                    case DjmainBeatmaniaColumnType.Player1FreeScratch:
                    {
                        if (command == DjmainEventType.SoundSelect)
                            ev[FlagData.Scratch] = true;
                        else
                            ev[FlagData.FreeZone] = true;

                        ev[NumericData.Player] = param0 & 1;
                        break;
                    }

                    default:
                    {
                        ev[NumericData.Column] = param0 >> 1;
                        ev[NumericData.Player] = param0 & 1;
                        break;
                    }
                }

                if (command == DjmainEventType.Marker && ev[FlagData.Measure] != true)
                    ev[FlagData.Note] = true;

                if (command == DjmainEventType.SoundSelect)
                    ev[NumericData.LoadSound] = param1 - 1;

                break;

            default:
                SetCommon(ev, command, param0, param1);
                break;
        }
    }

    public void AddPopnMetadata(Event ev, DjmainChartEvent ce)
    {
        var command = (DjmainEventType) (ce.Param0 & 0xF);
        var param0 = ce.Param0 >> 4;
        var param1 = ce.Param1;

        ev[NumericData.SourceCommand] = ce.Param0;
        ev[NumericData.SourceData] = ce.Param1;

        switch (command)
        {
            case DjmainEventType.Marker:
            case DjmainEventType.SoundSelect:
                ev[NumericData.Column] = param0;
                ev[NumericData.Player] = 0;
                ev[FlagData.Note] = true;

                if (command == DjmainEventType.SoundSelect)
                    ev[NumericData.LoadSound] = param1 - 1;

                break;

            default:
                SetCommon(ev, command, param0, param1);
                break;
        }
    }
}