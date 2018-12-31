using RhythmCodex.Infrastructure;
using RhythmCodex.Riff.Models;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Wav.Converters
{
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
            };
        }
    }

    public interface IWaveFmtDecoder
    {
        WaveFmtChunk Decode(IRiffChunk chunk);
    }
}