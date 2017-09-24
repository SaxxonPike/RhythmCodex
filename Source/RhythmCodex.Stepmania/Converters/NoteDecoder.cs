﻿using System.Collections.Generic;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public class NoteDecoder : INoteDecoder
    {
        public IEnumerable<IEvent> Decode(IEnumerable<Note> events, int columns)
        {
            return DecodeInternal(events, columns);
        }

        private IEnumerable<IEvent> DecodeInternal(IEnumerable<Note> events, int columns)
        {
            var holdHeadTypes = new Dictionary<int, FlagData>();

            foreach (var ev in events)
            {
                var result = new Event
                {
                    [NumericData.MetricOffset] = ev.MetricOffset,
                    [NumericData.SourceColumn] = ev.Column,
                    [NumericData.SourceData] = ev.Type,
                    [NumericData.Player] = ev.Column / columns,
                    [NumericData.Column] = ev.Column % columns
                };

                switch (ev.Type)
                {
                    case NoteType.Step:
                        result[FlagData.Note] = true;
                        break;
                    case NoteType.Freeze:
                        result[FlagData.Note] = true;
                        holdHeadTypes[ev.Column] = FlagData.Freeze;
                        break;
                    case NoteType.Tail:
                        if (holdHeadTypes.ContainsKey(ev.Column))
                        {
                            result[holdHeadTypes[ev.Column]] = true;
                            holdHeadTypes.Remove(ev.Column);
                        }
                        break;
                    case NoteType.Roll:
                        result[FlagData.Note] = true;
                        holdHeadTypes[ev.Column] = FlagData.Roll;
                        break;
                    case NoteType.Mine:
                        result[FlagData.Mine] = true;
                        break;
                    case NoteType.Lift:
                        result[FlagData.Lift] = true;
                        break;
                    case NoteType.Fake:
                        result[FlagData.Fake] = true;
                        break;
                }

                yield return result;
            }
        }
    }
}