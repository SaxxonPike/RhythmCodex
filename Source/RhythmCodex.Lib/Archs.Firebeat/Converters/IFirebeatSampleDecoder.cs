using System;
using System.Collections.Generic;
using RhythmCodex.Archs.Firebeat.Models;

namespace RhythmCodex.Archs.Firebeat.Converters;

public interface IFirebeatSampleDecoder
{
    Dictionary<int, FirebeatSample> Decode(
        ReadOnlySpan<byte> chunk,
        IEnumerable<KeyValuePair<int, FirebeatSampleInfo>> infos
    );
}