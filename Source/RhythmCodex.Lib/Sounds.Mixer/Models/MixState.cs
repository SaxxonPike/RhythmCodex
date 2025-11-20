using System;
using System.Linq;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Sounds.Mixer.Models;

public readonly record struct MixState(
    Sound? Sound,
    int SampleOffset,
    MixBalance? Balance,
    Metadata? EventData
)
{
    public int GetMaxLength()
    {
        if (Sound is not { Samples.Count: > 0 })
            return 0;

        var offset = SampleOffset;
        return Math.Max(0, Sound.Samples.Max(s => s.Data.Length - offset));
    }
}