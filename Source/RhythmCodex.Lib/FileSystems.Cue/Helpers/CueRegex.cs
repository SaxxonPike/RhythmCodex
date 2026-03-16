using System.Text.RegularExpressions;

namespace RhythmCodex.FileSystems.Cue.Helpers;

public static partial class CueRegex
{
    /// <summary>
    /// Matches mm:ss:ff time.
    /// </summary>
    [GeneratedRegex(@"(\d+):(\d+):(\d+)$")]
    public static partial Regex MinutesSecondsFrames();
}