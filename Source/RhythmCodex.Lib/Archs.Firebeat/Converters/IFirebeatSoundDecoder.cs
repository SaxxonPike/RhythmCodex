using System;
using System.Collections.Generic;
using RhythmCodex.Archs.Firebeat.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Firebeat.Converters;

public interface IFirebeatSoundDecoder
{
    Dictionary<int, Sound> Decode(IEnumerable<KeyValuePair<int, FirebeatSample>> samples);
    ReadOnlySpan<byte> TrimAudio(ReadOnlySpan<byte> data);
}