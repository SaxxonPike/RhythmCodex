using System;

namespace RhythmCodex.Xact.Processors;

public interface IFcs16Calculator
{
    short Calculate(ReadOnlySpan<byte> data);
}