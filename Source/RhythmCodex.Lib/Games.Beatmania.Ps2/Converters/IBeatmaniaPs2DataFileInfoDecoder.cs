using System;
using System.Collections.Generic;
using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

/// <summary>
/// Handles decoding file offsets and sizes from a beatmaniaIIDX PS2 game executable.
/// </summary>
public interface IBeatmaniaPs2DataFileInfoDecoder
{
    /// <summary>
    /// Decodes the file list for a given game executable.
    /// </summary>
    List<BeatmaniaPs2DataFileInfo> Decode(ReadOnlySpan<byte> data, int dataFileInfoOffset, BeatmaniaPs2FormatType type);
}