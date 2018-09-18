using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Converters;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Xa.Models;

namespace RhythmCodex.Xa.Converters
{
    public class XaDecoder : IXaDecoder
    {
        private readonly IDeinterleaver _deinterleaver;
        private readonly IXaFrameSplitter _xaFrameSplitter;
        
        // Reference: https://github.com/kode54/vgmstream/blob/master/src/coding/xa_decoder.c

        private static readonly int[] K0 = {0, 240, 460, 392};
        private static readonly int[] K1 = {0, 0, -208, -220};

        public XaDecoder(IDeinterleaver deinterleaver, IXaFrameSplitter xaFrameSplitter)
        {
            _deinterleaver = deinterleaver;
            _xaFrameSplitter = xaFrameSplitter;
        }

        public IList<ISound> Decode(XaChunk chunk)
        {
            var streams = _deinterleaver.Deinterleave(chunk.Data, chunk.Interleave, chunk.Channels);
            var sounds = new List<ISound>();
            var buffer = new float[28];
            var channels = 2;

            foreach (var stream in streams)
            {
                var states = Enumerable.Range(0, channels).Select(i => new XaState {Channel = i}).ToList();
                var samples = Enumerable.Range(0, channels).Select(i => new List<float>()).ToList();

                using (var mem = new MemoryStream(stream.AsArray()))
                using (var reader = new BinaryReader(mem))
                {
                    for (var offset = 0; offset < mem.Length - 0x7F; offset += 0x80)
                    {
                        var frame = reader.ReadBytes(0x80);
                        for (var c = 0; c < 8; c++)
                        {
                            DecodeFrame(frame, buffer, states[c % channels]);
                            samples[c % channels].AddRange(buffer);
                        }
                    }
                }

                for (var c = 0; c < channels; c += 2)
                {
                    sounds.Add(new Sound
                    {
                        Samples = new List<ISample>
                        {
                            new Sample {Data = samples[c]},
                            new Sample {Data = samples[c + 1]}
                        }
                    });
                }
            }

            return sounds;
        }

        private void DecodeFrame(byte[] frame, float[] buffer, XaState state)
        {
            var status = _xaFrameSplitter.GetStatus(frame, state.Channel);
            var magnitude = status & 0xF;
            var filter = status >> 4;
            var p1 = state.Prev1;
            var p2 = state.Prev2;
            var i = 0;

            foreach (var data in _xaFrameSplitter.GetData(frame, state.Channel))
            {
                var p0 = (data * (1 << (12 - magnitude)) +
                          K0[filter] * p1 +
                          K1[filter] * p2) >> 8;
                if (p0 > 32767)
                    p0 = 32767;
                else if (p0 < -32768)
                    p0 = -32768;
                buffer[i] = p0 / 32768f;
                p1 = p0;
                p2 = p1;
                i++;
            }

            state.Prev1 = p1;
            state.Prev2 = p2;
        }
    }
}