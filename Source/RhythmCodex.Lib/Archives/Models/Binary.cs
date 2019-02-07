using RhythmCodex.Attributes;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archives.Models
{
    [Model]
    public class Binary : Metadata, IBinary
    {
        public byte[] Data { get; set; }
    }
}