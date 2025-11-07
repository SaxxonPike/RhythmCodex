using System;
using System.Collections.Generic;
using RhythmCodex.Charts.Bms.Model;
using RhythmCodex.Charts.Models;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charts.Bms.Converters;

public interface IBmsNoteCommandEncoder
{
    string Encode(IEnumerable<BmsEvent> events, Func<BigRational?, string> encodeValue,
        BigRational measureLength, int quantize);

    List<BmsEvent> TranslateNoteEvents(IEnumerable<Event> events);
    List<BmsEvent> TranslateBpmEvents(IEnumerable<Event> chartEvents);
}