using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Xa.Models;

namespace RhythmCodex.Sounds.Xa.Converters;

[Service]
public class XaDecoder(IXaFrameSplitter xaFrameSplitter) : IXaDecoder
{
    // Reference: https://github.com/kode54/vgmstream/blob/master/src/coding/xa_decoder.c

    private static readonly int[] K0 = [0, 60, 115, 98, 122];
    private static readonly int[] K1 = [0, 0, -52, -55, -60];

    public List<Sound?> Decode(XaChunk chunk)
    {
        var sounds = new List<Sound?>();
        var buffer = new float[28];
        const int channels = 2;

        var states = Enumerable.Range(0, channels).Select(_ => new XaState()).ToList();
        var samples = Enumerable.Range(0, channels).Select(_ => new SampleBuilder()).ToList();

        using (var mem = new ReadOnlyMemoryStream(chunk.Data))
        using (var reader = new BinaryReader(mem))
        {
            for (var offset = 0; offset < mem.Length - 0x7F; offset += 0x80)
            {
                var frame = reader.ReadBytes(0x80);
                for (var c = 0; c < 8; c++)
                {
                    DecodeFrame(frame, buffer, c, states[c % channels]);
                    samples[c % channels].Append(buffer);
                }
            }
        }

        sounds.Add(new Sound
        {
            Samples = samples.Select(s => s.ToSample()).ToList()
        });

        return sounds;
    }

    private void DecodeFrame(ReadOnlySpan<byte> frame, Span<float> buffer, int channel, XaState state)
    {
        var status = xaFrameSplitter.GetStatus(frame, channel);
        var magnitude = status & 0x0F;
        if (magnitude < 0)
            magnitude = 3;
        magnitude += 16;
        var filter = (status & 0x30) >> 4;
        var k0 = K0[filter];
        var k1 = K1[filter];
        var p1 = state.Prev1;
        var p2 = state.Prev2;
        var dataBuffer = new int[28];
        xaFrameSplitter.Get4BitData(frame, dataBuffer, channel);

        for (var i = 0; i < 28; i++)
        {
            var delta = dataBuffer[i] << 28;
            var filter0 = k0 * p1;
            var filter1 = k1 * p2;
            var p0 = (delta >> magnitude) + ((filter0 + filter1) >> 6);
            p0 = p0 switch
            {
                > 32767 => 32767,
                < -32768 => -32768,
                _ => p0
            };
            buffer[i] = p0 / 32768f;
            p2 = p1;
            p1 = p0;
            i++;
        }

        state.Prev1 = p1;
        state.Prev2 = p2;
    }
}