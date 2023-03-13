using System.IO;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Heuristics;
using RhythmCodex.Vag.Models;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.Beatmania.Heuristics;

[Service]
public class BeatmaniaPs2NewBgmHeuristic : IReadableHeuristic<VagChunk>
{
    private readonly IVagStreamReader _vagStreamReader;

    public BeatmaniaPs2NewBgmHeuristic(IVagStreamReader vagStreamReader)
    {
        _vagStreamReader = vagStreamReader;
    }

    public string Description => "BeatmaniaIIDX CS BGM (new)";
    public string FileExtension => "bmcsbgm2";

    public HeuristicResult Match(IHeuristicReader reader)
    {
        if (reader.Length < 0x804)
            return null;

        var data = reader.Read(0x2C);
            
        if (Bitter.ToInt32(data.Slice(0x00)) != 0x08640001)
            return null;

        if (Bitter.ToInt32(data.Slice(0x04)) != 0)
            return null;

        var startOffset = Bitter.ToInt32(data.Slice(0x08));
        if (startOffset < 0x00000030)
            return null;

        var result = new VagHeuristicResult(this)
        {
            Start = startOffset,
            Length = Bitter.ToInt32(data.Slice(0x0C)),
            LoopStart = Bitter.ToInt32(data.Slice(0x10)),
            LoopEnd = Bitter.ToInt32(data.Slice(0x14)),
            SampleRate = Bitter.ToInt32(data.Slice(0x18)),
            Channels = Bitter.ToInt32(data.Slice(0x1C)),
            Interleave = Bitter.ToInt32(data.Slice(0x24)),
            Volume = new BigRational(Bitter.ToInt32(data.Slice(0x28)), 100),
        };

        if (data[0x20] != 0x00 || data[0x21] != 0x00 || data[0x22] != 0x00 || data[0x23] != 0x00)
            result.Key = data.Slice(0x20, 4).ToArray();

        return result;
    }

    public VagChunk Read(HeuristicResult result, Stream stream)
    {
        if (!(result is VagHeuristicResult info))
            return null;
            
        var decryptStream = info.Key != null
            ? new BeatmaniaPs2NewAudioDecryptStream(stream, info.Key)
            : stream;

        if (info.Start != null)
            stream.TryRead(0, (int) info.Start);

        var output = _vagStreamReader.Read(decryptStream, info.Channels ?? 1, info.Interleave ?? 0);
        output.Volume = info.Volume;
        output.SampleRate = info.SampleRate;
        output.Length = info.Length;

        return output;
    }
}