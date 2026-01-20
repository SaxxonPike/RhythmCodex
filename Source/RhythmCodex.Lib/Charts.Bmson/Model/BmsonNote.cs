using System.Text.Json.Serialization;

namespace RhythmCodex.Charts.Bmson.Model;

public class BmsonNote
{
    [JsonPropertyName("x")] public long X { get; set; }
    [JsonPropertyName("y")] public long Y { get; set; }
    [JsonPropertyName("l")] public long Length { get; set; }
    [JsonPropertyName("c")] public bool Continuation { get; set; }
}