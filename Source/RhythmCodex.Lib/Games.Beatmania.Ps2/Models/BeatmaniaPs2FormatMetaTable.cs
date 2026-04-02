namespace RhythmCodex.Games.Beatmania.Ps2.Models;

public struct BeatmaniaPs2FormatMetaTable
{
    public string? BinaryFileName { get; init; }
    public long FileTableOffset { get; init; }
    public long SongTableOffset { get; init; }
    public string? BlobFileName { get; init; }
    public long BlobOffset { get; init; }
    public int BaseIndex { get; init; }
    public int BaseSongIndex { get; init; }
}