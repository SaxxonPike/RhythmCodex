using System;
using System.IO;

namespace RhythmCodex.Infrastructure
{
    public static class Paths
    {
        public static string[] GetParts(string path) =>
            path.Split(new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar},
                StringSplitOptions.RemoveEmptyEntries);
    }
}