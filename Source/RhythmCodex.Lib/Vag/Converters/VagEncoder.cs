using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters
{
    [Service]
    public class VagEncoder : IVagEncoder
    {
        private readonly IVagDecrypter _vagDecrypter;

        public VagEncoder(IVagDecrypter vagDecrypter)
        {
            _vagDecrypter = vagDecrypter;
        }

        public VagChunk Encode(ISound sound, int? interleave = null)
        {
            var result = new VagChunk();
            var channels = sound.Samples.Count;

            if (channels == 0)
            {
                result.Data = new byte[0];
                return result;
            }

            if (channels > 1 && interleave == null)
                throw new RhythmCodexException("Interleave must be specified for multichannel input.");

            var allOutput = new List<byte[]>();
            var inter = ((interleave ?? 16) >> 4) << 4;
            var outBuffer = new byte[16];

            for (var channel = 0; channel < channels; channel++)
            {
                var compareIn = new float[28];
                var testIn = new int[28];
                var testOut = new float[28];
                var lastState = new VagDecodeState();
                var currentState = new VagDecodeState();
                var currentOut = new byte[16];
                var bestOut = new byte[16];
                var bestState = new VagDecodeState();
                var bestDiff = float.MaxValue;
                var inSampleOffset = 0;
                var lengthSamples = sound.Samples.Max(s => s.Data?.Count ?? 0);
                var output = new byte[(lengthSamples * 16 + 27) / 28];
                var sample = sound.Samples[channel].Data;
                var sampleCount = sample.Count;

                while (inSampleOffset < lengthSamples)
                {
                    // Quantize samples.
                    for (var i = 0; i < 28; i++)
                    {
                        if (i + inSampleOffset >= sampleCount)
                        {
                            testIn[i] = 0;
                            compareIn[i] = 0;
                        }
                        else
                        {
                            var y = sample[inSampleOffset + i];
                            var x = (int) Math.Round(y * 0x8000);
                            if (x > 0x7FFF)
                                x = 0x7FFF;
                            if (x < -0x8000)
                                x = -0x8000;
                            testIn[i] = x;
                            compareIn[i] = y;
                        }
                    }

                    // Shift samples.
                    var magnitude = 12;
                    while (magnitude > 0)
                    {
                        if (testIn.Any(i => i > 0x4000 || i < -0x4000))
                            break;
                        magnitude--;
                        for (var i = 0; i < 28; i++)
                            testIn[i] <<= 1;
                    }

                    // Build and test each filter.
                    currentOut[1] = 0x00;
                    for (var filter = 0; filter < 5; filter++)
                    {
                        currentOut[0] = unchecked((byte) (magnitude | (filter << 4)));
                        for (var i = 0; i < 28; i += 2)
                        {
                            currentOut[2 + (i >> 1)] =
                                unchecked((byte) (
                                    (testIn[i] >> 12) |
                                    (testIn[i + 1] >> 12)));
                        }

                        _vagDecrypter.Decrypt(currentOut, testOut, 16, currentState);
                    }

                    inSampleOffset += 28;
                }
            }

//            result.Data = output.ToArray();
//            result.Length = output.Length;
//            result.Interleave = inter;
//            result.Channels = channels;

            throw new Exception("Shit don't work, todo");
            return result;
        }
    }

    public interface IVagEncoder
    {
        VagChunk Encode(ISound sound, int? interleave = null);
    }
}