using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Wav.Converters
{
    [Service]
    public class PcmDecoder : IPcmDecoder
    {
        public float[] Decode8Bit(ReadOnlySpan<byte> bytes)
        {
            var result = new float[bytes.Length];
             for (var i = 0; i < bytes.Length; i++)
                result[i] = (bytes[i] / 128f) - 0.5f;
            return result;
        }

        public float[] Decode16Bit(ReadOnlySpan<byte> bytes)
        {
            var result = new float[bytes.Length / 2];
            for (int i = 0, j = 0; i < bytes.Length - 1; i += 2)
                result[j++] = (((bytes[i] << 16) | (bytes[i + 1] << 24)) >> 16) / 32768f;
            return result;
        }

        public float[] Decode24Bit(ReadOnlySpan<byte> bytes)
        {
            var result = new float[bytes.Length / 3];
            for (int i = 0, j = 0; i < bytes.Length - 2; i += 3)
                result[j++] = (((bytes[i] << 8) | (bytes[i + 1] << 16) | (bytes[i + 2] << 24)) >> 8) / 8388608f;
            return result;
        }

        public float[] Decode32Bit(ReadOnlySpan<byte> bytes)
        {
            var result = new float[bytes.Length / 4];
            for (int i = 0, j = 0; i < bytes.Length - 3; i += 4)
                result[j++] = (bytes[i] | (bytes[i + 1] << 8) | (bytes[i + 2] << 16) | (bytes[i + 3] << 24)) / 2147483648f;
            return result;
        }

        public float[] DecodeFloat(ReadOnlySpan<byte> bytes)
        {
            return MemoryMarshal.Cast<byte, float>(bytes).ToArray();
        }
    }
}