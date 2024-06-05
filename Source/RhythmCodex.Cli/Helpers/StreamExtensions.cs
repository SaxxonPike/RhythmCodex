using System;
using System.IO;
using JetBrains.Annotations;

namespace RhythmCodex.Cli.Helpers;

[PublicAPI]
internal static class StreamExtensions
{
    public static void WriteAllBytes(this ReadOnlySpan<byte> data, Stream stream)
    {
        stream.Write(data);
    }
        
    public static void WriteAllBytes(this Span<byte> data, Stream stream)
    {
        stream.Write(data);
    }
        
    public static void WriteAllBytes(this ReadOnlyMemory<byte> data, Stream stream)
    {
        stream.Write(data.Span);
    }
        
    public static void WriteAllBytes(this Memory<byte> data, Stream stream)
    {
        stream.Write(data.Span);
    }
        
    public static void WriteAllBytes(this byte[] data, Stream stream)
    {
        stream.Write(data);
    }
}