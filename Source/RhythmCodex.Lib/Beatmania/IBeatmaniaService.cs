using System.Collections.Generic;
using System.IO;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Beatmania;

/// <summary>
/// Handles Beatmania specific format conversion.
/// </summary>
public interface IBeatmaniaService
{
    /// <summary>
    /// Read a .1 chart file with the specified chart rate.
    /// </summary>
    List<IChart> ReadPcCharts(Stream stream, BigRational rate);

    /// <summary>
    /// Read a .1 chart file for any beatmaniaIIDX PC version prior to GOLD (14thstyle).
    /// This will use a chart rate of 59.94hz.
    /// </summary>
    List<IChart> ReadPcChartsBeforeGold(Stream stream) =>
        ReadPcCharts(stream, 59.94d);

    /// <summary>
    /// Read a .1 chart file for beatmaniaIIDX GOLD (14thstyle) PC.
    /// This will use a chart rate of 60.94hz.
    /// </summary>
    List<IChart> ReadPcChartsForGold(Stream stream) =>
        ReadPcCharts(stream, 60.94d);

    /// <summary>
    /// Read a .1 chart file for any beatmaniaIIDX PC version after GOLD (14thstyle).
    /// This will use a chart rate of 1000hz.
    /// </summary>
    List<IChart> ReadPcChartsAfterGold(Stream stream) =>
        ReadPcCharts(stream, 1000d);

    /// <summary>
    /// Read sounds from a .2DX file.
    /// </summary>
    List<ISound> ReadPcSounds(Stream stream);

    /// <summary>
    /// Read a chart from beatmaniaIIDX PS2 prior to 7thstyle, a format unofficially known as CS2.
    /// The chart rate is specified by the file itself.
    /// </summary>
    IChart ReadOldPs2Chart(Stream stream);
    
    /// <summary>
    /// Read a chart from beatmaniaIIDX PS2 7thstyle or later, a format unofficially known as CS.
    /// The chart rate is specified by the file itself.
    /// </summary>
    IChart ReadNewPs2Chart(Stream stream);
}