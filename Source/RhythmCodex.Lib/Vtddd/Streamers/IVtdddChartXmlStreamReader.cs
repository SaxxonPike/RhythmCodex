using System.Collections.Generic;
using System.IO;
using RhythmCodex.Vtddd.Models;

namespace RhythmCodex.Vtddd.Streamers;

public interface IVtdddChartXmlStreamReader
{
    List<VtdddStep> Read(Stream stream);
}