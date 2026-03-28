using System;
using RhythmCodex.Games.Beatmania.Psx.Models;

namespace RhythmCodex.Games.Beatmania.Psx.Converters;

/// <summary>
/// Determines the format of PSX Beatmania files using heuristics.
/// </summary>
public interface IPsxBeatmaniaFileFormatService
{
    /// <summary>
    /// Attempt to detect the format of a PSX Beatmania file.
    /// </summary>
    /// <param name="data">
    /// Data to scan.
    /// </param>
    /// <returns>
    /// The detected format, or <see cref="PsxBeatmaniaFileType.Unknown"/> if
    /// the format could not be determined.
    /// </returns>
    PsxBeatmaniaFileType DetectFormat(ReadOnlySpan<byte> data);
}