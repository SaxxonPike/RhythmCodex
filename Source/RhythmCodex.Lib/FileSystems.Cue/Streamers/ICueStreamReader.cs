using System.IO;
using RhythmCodex.FileSystems.Cue.Model;

namespace RhythmCodex.FileSystems.Cue.Streamers;

public interface ICueStreamReader
{
    CueFile ReadCue(Stream stream);
}