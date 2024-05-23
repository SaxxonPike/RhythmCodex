using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Vtddd.Models;

[Model]
public class VtdddDanceDb
{
    public List<VtdddDanceDbSong> Tracks { get; set; }
}