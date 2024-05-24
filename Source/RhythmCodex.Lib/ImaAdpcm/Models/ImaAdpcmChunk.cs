using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.ImaAdpcm.Models;

[Model]
public class ImaAdpcmChunk
{
    public required Memory<byte> Data { get; set; }
    public required int Channels { get; set; }
    public required int Rate { get; set; }
    public required int ChannelSamplesPerFrame { get; set; }
}