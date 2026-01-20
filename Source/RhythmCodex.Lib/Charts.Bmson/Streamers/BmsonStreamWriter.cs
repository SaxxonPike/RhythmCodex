using System.IO;
using System.Text.Json;
using RhythmCodex.Charts.Bmson.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Bmson.Streamers;

[Service]
public class BmsonStreamWriter : IBmsonStreamWriter
{
    public void Write(Stream target, BmsonFile file)
    {
        JsonSerializer.Serialize(target, file);
    }
}