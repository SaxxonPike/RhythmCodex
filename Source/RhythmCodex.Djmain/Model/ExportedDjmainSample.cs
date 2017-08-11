using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhythmCodex.Djmain.Model
{
    public struct ExportedDjmainSample
    {
        public DjmainSampleDefinition Definition { get; set; }
        public byte[] Data { get; set; }
    }
}
