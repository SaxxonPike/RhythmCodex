using RhythmCodex.Infrastructure;

namespace RhythmCodex.Xbox.Model
{
    [Model]
    public class XboxKasDefinition
    {
        public int Id { get; set; }
        public int Offset { get; set; }
        public int Length { get; set; }
    }
}