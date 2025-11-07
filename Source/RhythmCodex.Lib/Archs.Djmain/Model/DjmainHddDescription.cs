using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Djmain.Model;

public class DjmainHddDescription
{
    public DjmainChunkFormat Format { get; set; }

    public bool BytesAreSwapped { get; set; }

    public override string ToString() => Json.Serialize(this);
}