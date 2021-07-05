using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model
{
    [Model]
    public class XsbFile
    {
        public XsbHeader Header { get; set; }
        public XsbCue[] Cues { get; set; }
    }
}