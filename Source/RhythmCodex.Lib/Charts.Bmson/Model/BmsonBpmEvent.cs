using System.Text.Json.Serialization;

namespace RhythmCodex.Charts.Bmson.Model;

public class BmsonBpmEvent
{
    [JsonPropertyName("y")] public long Y { get; set; }
    [JsonPropertyName("bpm")] public double Bpm { get; set; }
}