using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.ImaAdpcm.Converters;

[Service]
public class ImaAdpcmDecoder : IImaAdpcmDecoder
{
    // Reference: https://github.com/dbry/adpcm-xq/blob/master/adpcm-lib.c

    public List<Sound?> Decode(ImaAdpcmChunk chunk)
    {
        var sounds = new List<Sound?>();
        var buffer = new float[chunk.ChannelSamplesPerFrame];
        var channels = chunk.Channels;
        var frameSize = chunk.ChannelSamplesPerFrame * chunk.Channels / 2 + chunk.Channels * 4;
        var max = chunk.Data.Length / frameSize * frameSize;
        var output = Enumerable.Range(0, channels).Select(_ => new List<float>()).ToArray();

        for (var offset = 0; offset < max; offset += frameSize)
        {
            var mem = chunk.Data.Span.Slice(offset, frameSize);
            for (var channel = 0; channel < channels; channel++)
            {
                DecodeFrame(mem, buffer, channel, channels);
                output[channel].AddRange(buffer);
            }
        }
            
        sounds.Add(new Sound
        {
            Samples = output.Select(s => new Sample {Data = s.ToArray()}).ToList()
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
                var step = ImaAdpcmConstants.StepTable[control];
                var delta = step >> 3;

                if ((data & 0x01) != 0) delta += step >> 2;
                if ((data & 0x02) != 0) delta += step >> 1;
                if ((data & 0x04) != 0) delta += step;
                if ((data & 0x08) != 0) delta = -delta;

                if (sample < -32768)
                    sample = -32768;
                if (sample > 32767)
                    sample = 32767;
                
                sample += delta;
                control += ImaAdpcmConstants.IndexTable[data & 0x7];
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