using System;
using System.Collections.Generic;
using RhythmCodex.Bms.Model;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bms.Converters;

public interface IBmsNoteCommandEncoder
{
    string Encode(IEnumerable<BmsEvent> events, Func<BigRational?, string> encodeValue,
        BigRational measureLength, int quantize);

    List<BmsEvent> TranslateNoteEvents(IEnumerable<Event> events);
    List<BmsEvent> TranslateBpmEvents(IEnumerable<Event> chartEvents);
}