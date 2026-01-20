using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RhythmCodex.Charts.Bmson.Model;

public class BmsonBga
{
    [JsonPropertyName("bga_header")] public List<BmsonBgaHeader> BgaHeader { get; set; } = [];
    [JsonPropertyName("bga_events")] public List<BmsonBgaEvent> BgaEvents { get; set; } = [];
    [JsonPropertyName("layer_events")] public List<BmsonBgaEvent> LayerEvents { get; set; } = [];
    [JsonPropertyName("poor_events")] public List<BmsonBgaEvent> PoorEvents { get; set; } = [];
}