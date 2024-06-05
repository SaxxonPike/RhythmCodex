using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model;

[Model]
public struct XsbSoundDsp
{
    public Memory<byte> ExtraData { get; set; }
}