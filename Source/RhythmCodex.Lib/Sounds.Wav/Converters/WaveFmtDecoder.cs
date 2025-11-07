using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Riff.Models;
using RhythmCodex.Sounds.Wav.Models;

namespace RhythmCodex.Sounds.Wav.Converters;

[Service]
public class WaveFmtDecoder : IWaveFmtDecoder
{
    public WaveFmtChunk Decode(RiffChunk chunk)
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