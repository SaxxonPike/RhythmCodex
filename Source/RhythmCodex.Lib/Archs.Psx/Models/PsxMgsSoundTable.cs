using System.Collections.Generic;

namespace RhythmCodex.Archs.Psx.Model;

public record PsxMgsSoundTable
{
    public List<PsxMgsSoundScript> Scripts { get; set; } = [];
}