using System;

namespace RhythmCodex.Sounds.Xact.Processors;

public interface IFcs16Calculator
{
    short Calculate(ReadOnlySpan<byte> data);
}