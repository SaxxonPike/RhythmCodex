using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Converters;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Vag.Models;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.Vag.Converters
{
    public class VagDecoder : IVagDecoder
    {
        private readonly IDeinterleaver _deinterleaver;

        public VagDecoder(IDeinterleaver deinterleaver)
        {
            _deinterleaver = deinterleaver;
        }

        public ISound Decode(VagChunk chunk)
        {
            using (var mem = new MemoryStream(chunk.Data))
            using (var stream = new VagStream(mem, new VagConfig
            {
                Channels = chunk.Channels,
                Interleave = chunk.Interleave,
                MaximumLength = chunk.Length ?? long.MaxValue
            }))
            {
                var data = stream.ReadAllBytes();
                var shorts = new short[data.Length / 2];
                Buffer.BlockCopy(data, 0, shorts, 0, shorts.Length * 2);
                var output = _deinterleaver
                    .Deinterleave(shorts.Select(s => (float) s / 32768f), 1, chunk.Channels)
                    .Select(floats => new Sample
                    {
                        Data = floats
                    })
                    .Cast<ISample>()
                    .ToArray();
                
                return new Sound
                {
                    Samples = output
                };
            }
        }
    }
}