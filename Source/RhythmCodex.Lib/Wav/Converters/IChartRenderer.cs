using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters;

public interface IChartRenderer
{
    Sound Render(IEnumerable<Event> events, IEnumerable<Sound> sounds, ChartRendererOptions options);
}