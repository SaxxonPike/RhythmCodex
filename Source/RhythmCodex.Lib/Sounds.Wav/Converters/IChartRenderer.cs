using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Wav.Models;

namespace RhythmCodex.Sounds.Wav.Converters;

public interface IChartRenderer
{
    Sound Render(IEnumerable<Event> events, IEnumerable<Sound> sounds, ChartRendererOptions options);
}