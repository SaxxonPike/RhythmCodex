using System;
using System.IO;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

public interface IBeatmaniaPs2OldChartStreamReader
{
    Memory<byte> Read(Stream stream, long length);
}