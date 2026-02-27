using System.Text.Json.Serialization;

namespace RhythmCodex.Charts.Bmson.Model;

public class BmsonBarLine
{
    [JsonPropertyName("y")] public long Y { get; set; }
}