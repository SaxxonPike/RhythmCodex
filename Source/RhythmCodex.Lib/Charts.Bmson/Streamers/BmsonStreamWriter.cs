using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using RhythmCodex.Charts.Bmson.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Bmson.Streamers;

[Service]
public class BmsonStreamWriter : IBmsonStreamWriter
{
    public void Write(Stream target, BmsonFile file)
    {
        JsonSerializer.Serialize(target, file, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    }
}