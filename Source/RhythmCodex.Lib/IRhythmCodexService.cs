using RhythmCodex.Arc;
using RhythmCodex.Beatmania;

namespace RhythmCodex;

/// <summary>
/// Exposes RhythmCodex tools as a single collection of services.
/// </summary>
public interface IRhythmCodexService
{
    IArcService Arc { get; }
    IBeatmaniaService Beatmania { get; }
}