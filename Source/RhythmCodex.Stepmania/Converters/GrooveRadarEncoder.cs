using RhythmCodex.Charting;

namespace RhythmCodex.Stepmania.Converters
{
    public class GrooveRadarEncoder : IGrooveRadarEncoder
    {
        public string Encode(IMetadata metadata)
        {
            var values = new[]
            {
                metadata[NotesCommandTag.StreamTag] ?? "0",
                metadata[NotesCommandTag.VoltageTag] ?? "0",
                metadata[NotesCommandTag.AirTag] ?? "0",
                metadata[NotesCommandTag.FreezeTag] ?? "0",
                metadata[NotesCommandTag.ChaosTag] ?? "0"
            };

            return string.Join(",", values);
        }
    }
}
