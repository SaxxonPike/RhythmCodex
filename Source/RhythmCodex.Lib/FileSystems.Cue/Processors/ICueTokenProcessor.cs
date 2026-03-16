using System.Collections.Generic;
using System.Text;

namespace RhythmCodex.FileSystems.Cue.Processors;

/// <summary>
/// Handles processing lines in a CUE file.
/// </summary>
public interface ICueTokenProcessor
{
    /// <summary>
    /// Breaks a CUE line up into individual tokens.
    /// </summary>
    /// <param name="line">
    /// Line to process.
    /// </param>
    /// <param name="sb">
    /// A string builder instance that will be reused for performance.
    /// Can be null if there is no shared StringBuilder instance.
    /// </param>
    /// <returns>
    /// A list of tokens in the CUE line.
    /// </returns>
    List<string> ProcessTokens(string line, StringBuilder? sb = null);

    /// <summary>
    /// Converts minutes:seconds:frames (mm:ss:ff) to a sector number.
    /// </summary>
    /// <param name="value">
    ///     Value to convert.
    /// </param>
    /// <returns>
    /// Converted value.
    /// </returns>
    int? ConvertMsfTimeToSector(string value);
}