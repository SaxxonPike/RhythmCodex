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
        private readonly IXaFrameSplitter _xaFrameSplitter;

        // Reference: https://github.com/kode54/vgmstream/blob/master/src/coding/xa_decoder.c

        private static readonly int[] K0 = {0, 60, 115, 98, 122};
        private static readonly int[] K1 = {0, 0, -52, -55, -60};

        public XaDecoder(IXaFrameSplitter xaFrameSplitter)
        {
            _xaFrameSplitter = xaFrameSplitter;
        }

        public IList<ISound> Decode(XaChunk chunk)
        {
            var sounds = new List<ISound>();
            var buffer = new float[28];
            var channels = 2;

            var states = Enumerable.Range(0, channels).Select(i => new XaState()).ToList();
            var samples = Enumerable.Range(0, channels).Select(i => new List<float>()).ToList();

            using (var mem = new MemoryStream(chunk.Data))
            using (var reader = new BinaryReader(mem))
            {
                for (var offset = 0; offset < mem.Length - 0x7F; offset += 0x80)
                {
                    var frame = reader.ReadBytes(0x80);
                    for (var c = 0; c < 8; c++)
                    {
                        DecodeFrame(frame, buffer, c, states[c % channels]);
                        samples[c % channels].AddRange(buffer);
                    }
                }
            }

            sounds.Add(new Sound
            {
                Samples = samples.Select(s => new Sample {Data = s}).Cast<ISample>().ToList()
            });

            return sounds;
        }

        private void DecodeFrame(byte[] frame, float[] buffer, int channel, XaState state)
        {
            var status = _xaFrameSplitter.GetStatus(frame, channel);
            var magnitude = status & 0x0F;
            if (magnitude < 0)
                magnitude = 3;
            magnitude += 16;
            var filter = (status & 0x30) >> 4;
            var k0 = K0[filter];
            var k1 = K1[filter];
            var p1 = state.Prev1;
            var p2 = state.Prev2;
            var i = 0;

            foreach (var data in _xaFrameSplitter.Get4BitData(frame, channel))
            {
                var delta = data << 28;
                var filter0 = k0 * p1;
                var filter1 = k1 * p2;
                var p0 = (delta >> magnitude) + ((filter0 + filter1) >> 6);
                if (p0 > 32767)
                    p0 = 32767;
                else if (p0 < -32768)
                    p0 = -32768;
                buffer[i] = p0 / 32768f;
                p2 = p1;
                p1 = p0;
                i++;
            }

            state.Prev1 = p1;
            state.Prev2 = p2;
        }
    }
}