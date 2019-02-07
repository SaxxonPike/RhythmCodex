using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;

namespace RhythmCodex.Archives.Models
{
    [Model]
    public class Binary : Metadata, IBinary
    {
        public byte[] Data { get; set; }
    }
}