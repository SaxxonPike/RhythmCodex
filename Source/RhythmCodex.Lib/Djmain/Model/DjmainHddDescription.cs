using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

public record DjmainHddDescription
{
    public DjmainChunkFormat Format { get; init; }

    public bool BytesAreSwapped { get; init; }

    public override string ToString() => Json.Serialize(this);
}