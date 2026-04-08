using System;
using RhythmCodex.FileSystems.Cd.Model;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.FileSystems.Cd.Helpers;

public static class CdFileExtensions
{
    public static Memory<byte> ReadFile(this ICdFile cdFile)
    {
        using var stream = cdFile.Open();
        return stream.ReadAllBytes();
    }
}