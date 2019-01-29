using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Riff.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters
{
    [Service]
    public class WaveFmtDecoder : IWaveFmtDecoder
    {
        public WaveFmtChunk Decode(IRiffChunk chunk)
        {
            var data = chunk.Data;
            
            return new WaveFmtChunk
            {
                Format = Bitter.ToInt16(data, 0),
                Channels = Bitter.ToInt16(data, 2),
                SampleRate = Bitter.ToInt32(data, 4),
                ByteRate = Bitter.ToInt32(data, 8),
                BlockAlign = Bitter.ToInt16(data, 12),
                BitsPerSample = Bitter.ToInt16(data, 14),
                ExtraData = data.Length > 16 ? data.AsSpan(16).ToArray() : new byte[0]
            };
        }
    }

    public interface IWaveFmtDecoder
    {
        WaveFmtChunk Decode(IRiffChunk chunk);
    }
}