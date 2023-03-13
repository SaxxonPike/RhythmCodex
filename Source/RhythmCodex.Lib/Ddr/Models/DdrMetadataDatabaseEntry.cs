namespace RhythmCodex.Ddr.Models;

public class DdrMetadataDatabaseEntry
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Artist { get; set; }
    public string TitleRoman { get; set; }
    public string SubtitleRoman { get; set; }
    public string ArtistRoman { get; set; }
    public string TitleLocal { get; set; }
    public string SubtitleLocal { get; set; }
    public string ArtistLocal { get; set; }
}