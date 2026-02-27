using System.IO;
using RhythmCodex.Charts.Bmson.Model;

namespace RhythmCodex.Charts.Bmson.Streamers;

public interface IBmsonStreamReader
{
    BmsonFile Read(Stream source);
}