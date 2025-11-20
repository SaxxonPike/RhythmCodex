using System.IO;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Vag.Heuristics;
using RhythmCodex.Sounds.Vag.Models;
using RhythmCodex.Sounds.Vag.Streamers;

namespace RhythmCodex.Games.Beatmania.Ps2.Heuristics;

[Service]
[Context(Context.BeatmaniaIidxCs)]
public class BeatmaniaPs2OldBgmHeuristic(IVagStreamReader vagStreamReader) : IReadableHeuristic<VagChunk>
{
    public string Description => "BeatmaniaIIDX CS BGM (old)";
    public string FileExtension => "bmcsbgm";
        
    public HeuristicResult? Match(IHeuristicReader reader)
    {
        var data = reader.Read(0x10);

        if (data.Length < 0x10)
            return null;

        var length = Bitter.ToInt32S(data);

        if (length == 0)
            return null;

        if ((length & 0x7FF) != 0)
            return null;

        if (data[0x05] == 0)
            return null;

        var sampleRate = Bitter.ToInt16S(data, 6);
        if (sampleRate == 0)
            return null;

        if (data[0x08] == 0)
            return null;

        if (data[0x08] > 2)
            return null;

        return new VagHeuristicResult(this)
        {
            Start = 0x800,
            Interleave = 0x800,
            Channels = data[0x08],
            SampleRate = sampleRate,
            Length = length
        };
    }

    public VagChunk? Read(HeuristicResult result, Stream stream)
    {
        var info = result as VagHeuristicResult;
        return vagStreamReader.Read(stream, info?.Channels ?? 1, info?.Interleave ?? 0);
    }
}