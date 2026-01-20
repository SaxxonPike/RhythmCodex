using System.IO;
using System.Text.Json;
using RhythmCodex.Charts.Bmson.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Bmson.Streamers;

[Service]
public class BmsonStreamReader : IBmsonStreamReader
{
    public BmsonFile Read(Stream source) =>
        JsonSerializer.Deserialize<BmsonFile>(source) ??
        throw new RhythmCodexException("Failed to read Bmson file.");
}