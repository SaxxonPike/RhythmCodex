using RhythmCodex.FileSystems.Arc;
using RhythmCodex.Games.Beatmania;

namespace RhythmCodex;

/// <summary>
/// Exposes RhythmCodex tools as a single collection of services.
/// </summary>
public interface IRhythmCodexService
{
    IArcService Arc { get; }
    IBeatmaniaService Beatmania { get; }
}