using System;
using System.Collections.Generic;
using RhythmCodex.Bms.Model;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bms.Converters
{
    public interface IBmsNoteCommandEncoder
    {
        string Encode(IEnumerable<BmsEvent> events, Func<BigRational, string> encodeValue,
            BigRational measureLength, int quantize);

        IEnumerable<BmsEvent> TranslateNoteEvents(IEnumerable<IEvent> events);
        IEnumerable<BmsEvent> TranslateBpmEvents(IEnumerable<IEvent> chartEvents);
    }
}