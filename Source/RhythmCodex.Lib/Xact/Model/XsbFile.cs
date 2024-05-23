using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model;

[Model]
public class XsbFile
{
    public required XsbHeader Header { get; init; }
    public required List<XsbCue> Cues { get; init; }
}