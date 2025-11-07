using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sounds.Xact.Model;

[Model]
public class XsbFile
{
    public XsbHeader Header { get; set; }
    public List<XsbCue> Cues { get; set; } = [];
}