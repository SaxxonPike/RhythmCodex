using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Xa.Models;

namespace RhythmCodex.ImaAdpcm.Converters
{
    [Service]
    public class ImaAdpcmDecoder : IImaAdpcmDecoder
    {
        // Reference: https://github.com/dbry/adpcm-xq/blob/master/adpcm-lib.c

        private static readonly int[] StepTable =
        {
            7, 8, 9, 10, 11, 12, 13, 14,
            16, 17, 19, 21, 23, 25, 28, 31,
            34, 37, 41, 45, 50, 55, 60, 66,
            73, 80, 88, 97, 107, 118, 130, 143,
            157, 173, 190, 209, 230, 253, 279, 307,
            337, 371, 408, 449, 494, 544, 598, 658,
            724, 796, 876, 963, 1060, 1166, 1282, 1411,
            1552, 1707, 1878, 2066, 2272, 2499, 2749, 3024,
            3327, 3660, 4026, 4428, 4871, 5358, 5894, 6484,
            7132, 7845, 8630, 9493, 10442, 11487, 12635, 13899,
            15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794,
            32767
        };

        private static readonly int[] IndexTable =
        {
            -1, -1, -1, -1, 2, 4, 6, 8
        };

        public IList<ISound> Decode(ImaAdpcmChunk chunk)
        {
            var sounds = new List<ISound>();
            var buffer = new float[chunk.ChannelSamplesPerFrame];
            var channels = chunk.Channels;
            var frameSize = (chunk.ChannelSamplesPerFrame * chunk.Channels / 2) + (chunk.Channels * 4);
            var max = (chunk.Data.Length / frameSize) * frameSize;
            var output = Enumerable.Range(0, channels).Select(i => new List<float>()).ToArray();

            for (var offset = 0; offset < max; offset += frameSize)
            {
                var mem = chunk.Data.AsSpan(offset, frameSize);
                for (var channel = 0; channel < channels; channel++)
                {
                    DecodeFrame(mem, buffer, channel, channels);
                    output[channel].AddRange(buffer);
                }
            }
            
            sounds.Add(new Sound
            {
                Samples = output.Select(s => new Sample {Data = s}).Cast<ISample>().ToList()
            });

            return sounds;            
        }

        private int DecodeFrame(ReadOnlySpan<byte> frame, Span<float> buffer, int channel, int channelCount)
        {
            var index = channel << 2;
            var sample = ((frame[index] | (frame[index + 1] << 8)) << 16) >> 16;
            int control = frame[index + 2];
            var max = frame.Length;
            var bufferIndex = 0;
            index = channelCount << 2;
            var nybbleIndex = 0;
            var channelIndex = 0;

            while (index < max)
            {
                float DecodeNybble(int data)
                {
                    var step = StepTable[control];
                    var delta = step >> 3;

                    if ((data & 0x01) != 0) delta += (step >> 2);
                    if ((data & 0x02) != 0) delta += (step >> 1);
                    if ((data & 0x04) != 0) delta += step;
                    if ((data & 0x08) != 0) delta = -delta;

                    if (sample < -32768)
                        sample = -32768;
                    if (sample > 32767)
                        sample = 32767;
                
                    sample += delta;
                    control += IndexTable[data & 0x7];
                    if (control < 0)
                        control = 0;
                    if (control > 88)
                        control = 88;
                    return sample / 32768f;
                }

                if (nybbleIndex == 8)
                {
                    nybbleIndex = 0;
                    channelIndex++;
                    if (channelIndex == channelCount)
                        channelIndex = 0;
                }
                
                if (channelIndex == channel)
                    buffer[bufferIndex++] = DecodeNybble(frame[index]);
                nybbleIndex++;

                if (channelIndex == channel)
                    buffer[bufferIndex++] = DecodeNybble(frame[index] >> 4);
                nybbleIndex++;
                
                index++;
            }

            return bufferIndex;
        }
    }
}