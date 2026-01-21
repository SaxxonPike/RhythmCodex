using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RhythmCodex.Charts.Bmson.Model;

public class BmsonFile
{
    [JsonPropertyName("version")] public string? Version { get; set; }
    [JsonPropertyName("info")] public BmsonInfo Info { get; set; } = new();
    [JsonPropertyName("lines")] public List<BmsonBarLine> Lines { get; set; } = [];
    [JsonPropertyName("bpm_events")] public List<BmsonBpmEvent> BpmEvents { get; set; } = [];
    [JsonPropertyName("stop_events")] public List<BmsonStopEvent> StopEvents { get; set; } = [];
    [JsonPropertyName("sound_channels")] public List<BmsonSoundChannel> SoundChannels { get; set; } = [];
    [JsonPropertyName("bga")] public BmsonBga Bga { get; set; } = new();
}