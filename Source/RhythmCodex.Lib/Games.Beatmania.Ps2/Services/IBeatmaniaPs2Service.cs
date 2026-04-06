using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Services;

/// <summary>
/// Provides a simple interface for beatmaniaIIDX PS2 game operations.
/// </summary>
public interface IBeatmaniaPs2Service
{
    /// <summary>
    /// Decodes chart sets from a file system.
    /// </summary>
    /// <param name="openFile">
    /// Function that will be used to open files.
    /// </param>
    /// <param name="type">
    /// Game type that will be used to determine source file names.
    /// </param>
    /// <returns>
    /// Decoded chart sets.
    /// </returns>
    /// <remarks>
    /// The return value from this method uses deferred execution and streams
    /// the data from the file system as-needed. This is because the result data
    /// can get quite large.
    /// </remarks>
    IEnumerable<BeatmaniaPs2ChartSet> Decode(Func<string, Stream> openFile,
        BeatmaniaPs2FormatType type);
}