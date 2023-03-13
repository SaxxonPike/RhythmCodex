using RhythmCodex.Infrastructure;

namespace RhythmCodex.Vtddd.Models;

[Model]
public class VtdddDanceDbSong
{
    public int SongId { get; set; }

    public string Artist { get; set; }
    public string Title { get; set; }
    public int? DifficultyEasy { get; set; }
    public int? DifficultyMedium { get; set; }
    public int? DifficultyHard { get; set; }
    public int? DifficultyExpert { get; set; }
    public int? Silliness { get; set; }
    public int? BpmEasy { get; set; }
    public int? BpmMedium { get; set; }
    public int? BpmHard { get; set; }
    public int? AlbumId { get; set; }
    public string Wave { get; set; }
    public string Lyrics { get; set; }
    public string ChartEasy { get; set; }
    public string ChartMedium { get; set; }
    public string ChartHard { get; set; }
    public string ChartExpert { get; set; }
}