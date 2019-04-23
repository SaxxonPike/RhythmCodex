using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters
{
    [Service]
    public class VagDecoder : IVagDecoder
    {
        private readonly IVagSplitter _vagSplitter;

        public VagDecoder(IVagSplitter vagSplitter)
        {
            _vagSplitter = vagSplitter;
        }
        
        public ISound Decode(VagChunk chunk)
        {
            return new Sound
            {
                Samples = _vagSplitter.Split(chunk)
            };
        }
    }
}