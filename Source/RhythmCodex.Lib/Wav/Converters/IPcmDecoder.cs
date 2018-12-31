using System.Collections.Generic;

namespace RhythmCodex.Wav.Converters
{
    public interface IPcmDecoder
    {
        float[] Decode8Bit(byte[] bytes);
        float[] Decode16Bit(byte[] bytes);
        float[] Decode24Bit(byte[] bytes);
        float[] Decode32Bit(byte[] bytes);
        float[] DecodeFloat(byte[] bytes);
    }
}