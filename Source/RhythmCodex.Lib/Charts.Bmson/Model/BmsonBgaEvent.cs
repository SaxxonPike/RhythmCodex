using System.Text.Json.Serialization;

namespace RhythmCodex.Charts.Bmson.Model;

public class BmsonBgaEvent
{
    [JsonPropertyName("y")] public long Y { get; set; }
    [JsonPropertyName("id")] public long Id { get; set; }
}