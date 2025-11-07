using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Vtddd.Models;

namespace RhythmCodex.Games.Vtddd.Streamers;

public interface IVtdddChartXmlStreamReader
{
    List<VtdddStep> Read(Stream stream);
}