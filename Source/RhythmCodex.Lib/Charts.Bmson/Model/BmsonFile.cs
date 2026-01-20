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
    //
    // public static BmsonFile? Load(ReadOnlySpan<byte> content) =>
    //     content.Length == 0
    //         ? null 
    //         : JsonSerializer.Deserialize<BmsonFile>(content);
    //
    // public static BmsonFile? Load(ReadOnlySpan<char> content) =>
    //     content.Length == 0
    //         ? null 
    //         : JsonSerializer.Deserialize<BmsonFile>(content);
    //
    // public static BmsonFile? Load(Stream stream) =>
    //     stream is { CanSeek: true, Length: 0 }
    //         ? null 
    //         : JsonSerializer.Deserialize<BmsonFile>(stream);
    //
    // public void Save(Stream stream) =>
    //     JsonSerializer.Serialize(stream, this);
}