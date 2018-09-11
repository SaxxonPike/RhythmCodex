using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var channelData = _deinterleaver.Deinterleave(chunk.Data, chunk.Interleave, chunk.Channels);
            var samples = channelData.Select(d =>
            {
                    
                using (var mem = new MemoryStream(chunk.Data))
                using (var stream = new VagStream(mem, new VagConfig { Channels = 1}))
                {
                    var data = stream.ReadAllBytes();
                    var shorts = new short[data.Length / 2];
                    Buffer.BlockCopy(data, 0, shorts, 0, shorts.Length * 2);
                    
                    return new Sample
                    {
                        Data = shorts.Select(s => (float)s / 32768f).ToArray()
                    };
                }
            }).Cast<ISample>().ToList();
            
            return new Sound
            {
                Samples = samples
            };
        }
    }
}