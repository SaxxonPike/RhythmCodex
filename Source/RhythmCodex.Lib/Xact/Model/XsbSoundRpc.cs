﻿using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xact.Model;

[Model]
public struct XsbSoundRpc
{
    public int[] Curves { get; set; }
    public Memory<byte> ExtraData { get; set; }
}