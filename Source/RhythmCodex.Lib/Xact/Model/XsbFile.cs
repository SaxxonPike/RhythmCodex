using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model;

[Model]
public class XsbFile
{
    public XsbHeader Header { get; init; }
    public List<XsbCue> Cues { get; init; }
}