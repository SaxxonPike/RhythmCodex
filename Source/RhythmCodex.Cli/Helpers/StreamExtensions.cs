using System;
using System.IO;

namespace RhythmCodex.Cli.Helpers;

public static class StreamExtensions
{
    private const int BufferSize = 4096;

    public static void WriteAllBytes(this ReadOnlySpan<byte> data, Stream stream)
    {
        stream.Write(data);
    }
        
    public static void WriteAllBytes(this byte[] data, Stream stream)
    {
        stream.Write(data);
    }
}