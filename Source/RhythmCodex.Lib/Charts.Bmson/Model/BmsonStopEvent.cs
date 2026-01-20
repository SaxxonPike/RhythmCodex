using System.Text.Json.Serialization;

namespace RhythmCodex.Charts.Bmson.Model;

public class BmsonStopEvent
{
    [JsonPropertyName("y")] public long Y { get; set; }
    [JsonPropertyName("duration")] public long Duration { get; set; }
}