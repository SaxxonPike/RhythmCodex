using System;
using System.IO;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Models;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.Vag.Heuristics;

[Service]
public class SvagHeuristic : IReadableHeuristic<SvagContainer>
{
    private readonly IVagStreamReader _vagStreamReader;

    public SvagHeuristic(IVagStreamReader vagStreamReader)
    {
        _vagStreamReader = vagStreamReader;
    }
        
    public SvagContainer Read(HeuristicResult result, Stream stream)
    {
        if (!(result is VagHeuristicResult info))
            return null;
            
        if (info.Start != null)
            stream.TryRead(0, (int) info.Start);

        var output = _vagStreamReader.Read(stream, info.Channels ?? 1, info.Interleave ?? 0);
        output.Volume = info.Volume;
        output.SampleRate = info.SampleRate;
        output.Length = info.Length;

        return new SvagContainer
        {
            SampleRate = info.SampleRate,
            VagChunk = output
        };
    }

    public string Description => "SVAG sample";

    public string FileExtension => "svag";
        
    public HeuristicResult Match(IHeuristicReader reader)
    {
        Span<int> words = stackalloc int[7];
        Span<byte> data = stackalloc byte[0x1C];

        if (reader.Read(data) < 0x1C)
            return null;

        Bitter.ToInt32Values(data, words);
            
        // "Svag"
        if (words[0] != 0x67617653)
            return null;
            
        // Make sure length is 16-byte aligned
        if ((words[1] & 0xF) != 0)
            return null;
            
        // Make sure sample rate fits in 16bits
        if (words[2] > 0xFFFF || words[2] <= 0)
            return null;
            
        // More than 4 channels is pretty unlikely
        if (words[3] < 1 || words[3] > 4)
            return null;

        // Total length must be at least the header size
        if (words[1] < 0x800)
            return null;

        // Interleave must be non negative
        if (words[4] < 0)
            return null;

        // Interleave must be present if more than 1 channel
        if (words[3] > 1 && words[4] == 0)
            return null;
            
        // Must have at least 1 full block present
        if (words[1] < words[3] * words[4])
            return null;
            
        // The usable header length is 0x1C but the last two words are loop info, we don't care about it
            
        return new VagHeuristicResult(this)
        {
            Start = 0x800,
            Length = words[1] - 0x800,
            SampleRate = words[2],
            Channels = words[3],
            Interleave = words[4],
            LoopStart = words[5],
            LoopEnd = words[6]
        };
    }
}