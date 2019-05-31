using System;
using RhythmCodex.Charting.Models;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Djmain.Converters
{
    [Service]
    public class DjmainEventMetadataDecoder : IDjmainEventMetadataDecoder
    {
        private void SetCommon(IEvent ev, DjmainEventType command, int param0, int param1)
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
                    ev[NumericData.Panning] = new BigRational(Math.Max(param0 - 1, 0), 14);
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

        public void AddBeatmaniaMetadata(IEvent ev, IDjmainChartEvent ce)
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
                            ev[FlagData.Note] = true;
                            ev[NumericData.Player] = param0 & 1;
                            break;
                        }

                        case DjmainBeatmaniaColumnType.Player0Measure:
                        case DjmainBeatmaniaColumnType.Player1Measure:
                        {
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
                            ev[FlagData.Note] = true;
                            break;
                        }
                    }

                    if (command == DjmainEventType.SoundSelect)
                        ev[NumericData.LoadSound] = param1 - 1;

                    break;

                default:
                    SetCommon(ev, command, param0, param1);
                    break;
            }
        }

        public void AddPopnMetadata(IEvent ev, IDjmainChartEvent ce)
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
}