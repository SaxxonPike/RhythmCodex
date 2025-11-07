using System;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Ddr.Models;

[Model]
public class DdrDatabaseEntry
{
    public int Index { get; set; }
    public string? Id { get; set; }
    public int Type { get; set; }
    public int CdTitle { get; set; }
    public int InternalId { get; set; }
    public int MaxBpm { get; set; }
    public int MinBpm { get; set; }
    public int Unknown014 { get; set; }
    public int SonglistOrder { get; set; }
    public int UnlockNumber { get; set; }
    public int[] Difficulties { get; set; } = [];
    public int Unknown01E { get; set; }
    public int Unknown022 { get; set; }
    public int Flags { get; set; }
    public int[] Radar0 { get; set; } = [];
    public int[] Radar1 { get; set; } = [];
    public int[] Radar2 { get; set; } = [];
    public int[] Radar3 { get; set; } = [];
    public int[] Radar4 { get; set; } = [];
    public string? LongName { get; set; }
    public string? ShortName { get; set; }
    public int AudioTrack { get; set; }

    public override string ToString()
    {
        return $"db[idx={Index:D4} " +
               $"int={InternalId:D3} " +
               $"aud={AudioTrack:D3}] " +
               $"Id:{Id} " +
               $"Long:{LongName ?? ""} " +
               $"Short:{ShortName ?? ""} " +
               $"BPM:{MinBpm}-{MaxBpm} " +
               $"Diff:[{string.Join(",", Difficulties?.Select(d => $"{d}") ?? Array.Empty<string>())}]";
    }
}