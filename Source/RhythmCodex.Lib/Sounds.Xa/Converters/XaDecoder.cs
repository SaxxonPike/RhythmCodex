using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Models;
using RhythmCodex.Sounds.Xa.Models;

namespace RhythmCodex.Sounds.Xa.Converters;

[Service]
public class XaDecoder(IXaFrameSplitter xaFrameSplitter) : IXaDecoder
{
    // Reference: https://github.com/kode54/vgmstream/blob/master/src/coding/xa_decoder.c

    private static readonly int[] K0 = VagCoefficients.Coeff0;
    private static readonly int[] K1 = VagCoefficients.Coeff1;

    public List<Sound?> Decode(XaChunk chunk)
    {
        var sounds = new List<Sound?>();
        Span<float> buffer = stackalloc float[28];
        //const int channels = 2;
        var channels = chunk.Channels;

        var states = Enumerable.Range(0, channels).Select(_ => new XaState()).ToList();
        var samples = Enumerable.Range(0, channels).Select(_ => new SampleBuilder()).ToList();
        var data = chunk.Data.Span;

        for (var offset = 0; offset < data.Length - 0x7F; offset += 0x80)
        {
            var frame = data.Slice(offset, 0x80);

            for (var c = 0; c < 8; c++)
            {
                var ch = c % channels;
                DecodeFrame(frame, buffer, c, states[ch]);
                samples[ch].Append(buffer);
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
        Span<byte> dataBuffer = new byte[28];
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
        }

        state.Prev1 = p1;
        state.Prev2 = p2;
    }
}