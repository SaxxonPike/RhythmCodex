namespace RhythmCodex.Archs.Psx.Model;

public record PsxBeatmaniaFileEntry
{
    public int Index { get; set; }
    public int Offset { get; set; }
    public int Length { get; set; }
    public long BaseOffset { get; set; }
}