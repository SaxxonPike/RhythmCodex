using System.IO;
using RhythmCodex.Beatmania.Ps2.Models;
using RhythmCodex.Beatmania.Ps2.Streamers;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Ps2.Heuristics;

[Service]
public class BeatmaniaPs2NewKeysoundHeuristic(IBeatmaniaPs2NewKeysoundStreamReader keysoundReader)
    : IReadableHeuristic<BeatmaniaPs2KeysoundSet>
{
    public string Description => "BeatmaniaIIDX CS Keysounds (new)";
    public string FileExtension => "bmcsbgm2";

    public HeuristicResult? Match(IHeuristicReader reader)
    {
        if (reader.Length < 0x10)
            return null;

        var totalSize = 0;
        var instrumentHeader = reader.Read(0x10);

        if (Bitter.ToInt32(instrumentHeader) != 0x00016665)
            return null;

        var instrumentBlockCount = Bitter.ToInt32(instrumentHeader[0x4..]);
        if (instrumentBlockCount * 0x800 + 0x10 > reader.Length)
            return null;

        totalSize += instrumentBlockCount * 0x800;
        reader.Skip(instrumentBlockCount * 0x800 - 0x10);

        var sampleHeader = reader.Read(0x10);
        var sampleWaveSize = Bitter.ToInt32(sampleHeader[0x4..]);
        var sampleCompositeSize = Bitter.ToInt32(sampleHeader) * 0x10 + sampleWaveSize + 0x10;
        totalSize += sampleCompositeSize;

        if (totalSize > reader.Length)
            return null;

        var result = new BeatmaniaPs2NewKeysoundHeuristicResult(this)
        {
            Length = totalSize
        };

        return result;
    }

    public BeatmaniaPs2KeysoundSet? Read(HeuristicResult result, Stream stream)
    {
        if (result is not BeatmaniaPs2NewKeysoundHeuristicResult)
            return null;

        return keysoundReader.Read(stream);
    }
}