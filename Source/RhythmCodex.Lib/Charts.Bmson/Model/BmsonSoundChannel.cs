using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RhythmCodex.Charts.Bmson.Model;

public class BmsonSoundChannel
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("notes")] public List<BmsonNote> Notes { get; set; } = [];
}