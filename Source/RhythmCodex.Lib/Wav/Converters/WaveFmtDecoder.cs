using System;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Riff.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters;

[Service]
public class WaveFmtDecoder : IWaveFmtDecoder
{
    public WaveFmtChunk Decode(IRiffChunk chunk)
    {
        var data = chunk.Data;
            
        return new WaveFmtChunk
        {
            Format = Bitter.ToInt16(data.Span, 0),
            Channels = Bitter.ToInt16(data.Span, 2),
            SampleRate = Bitter.ToInt32(data.Span, 4),
            ByteRate = Bitter.ToInt32(data.Span, 8),
            BlockAlign = Bitter.ToInt16(data.Span, 12),
            BitsPerSample = Bitter.ToInt16(data.Span, 14),
            ExtraData = data.Length > 16 ? data.Span[16..].ToArray() : Memory<byte>.Empty
        };
    }
}