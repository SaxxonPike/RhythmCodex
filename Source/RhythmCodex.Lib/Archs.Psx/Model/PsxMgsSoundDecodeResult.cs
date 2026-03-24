using System.Collections.Generic;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Psx.Model;

public struct PsxMgsSoundDecodeResult
{
    public int Index { get; init; }
    public Sound Sound { get; init; }
    public Dictionary<int, List<PsxMgsSoundTablePacket>> Packets { get; init; }
}