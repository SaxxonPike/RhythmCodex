using System.Collections.Generic;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters
{
    public interface IChartRenderer
    {
        ISound Render(IEnumerable<IEvent> events, IEnumerable<ISound> sounds, ChartRendererOptions options);
    }
}