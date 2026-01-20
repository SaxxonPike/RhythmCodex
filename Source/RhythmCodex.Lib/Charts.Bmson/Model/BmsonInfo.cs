using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RhythmCodex.Charts.Bmson.Model;

public class BmsonInfo
{
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("subtitle")] public string? SubTitle { get; set; }
    [JsonPropertyName("artist")] public string? Artist { get; set; }
    [JsonPropertyName("subartists")] public List<string>? SubArtists { get; set; }
    [JsonPropertyName("genre")] public string? Genre { get; set; }
    [JsonPropertyName("mode_hint")] public string? ModeHint { get; set; }
    [JsonPropertyName("chart_name")] public string? ChartName { get; set; }
    [JsonPropertyName("level")] public long Level { get; set; }
    [JsonPropertyName("init_bpm")] public double InitBpm { get; set; }
    [JsonPropertyName("judge_rank")] public double JudgeRank { get; set; }
    [JsonPropertyName("total")] public double Total { get; set; }
    [JsonPropertyName("back_image")] public string? BackImage { get; set; }
    [JsonPropertyName("eyecatch_image")] public string? EyeCatchImage { get; set; }
    [JsonPropertyName("banner_image")] public string? BannerImage { get; set; }
    [JsonPropertyName("preview_music")] public string? PreviewMusic { get; set; }
    [JsonPropertyName("resolution")] public long Resolution { get; set; }
}