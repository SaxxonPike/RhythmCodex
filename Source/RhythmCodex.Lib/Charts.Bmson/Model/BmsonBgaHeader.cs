using System.Text.Json.Serialization;

namespace RhythmCodex.Charts.Bmson.Model;

public class BmsonBgaHeader
{
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
}