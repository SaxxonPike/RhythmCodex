namespace RhythmCodex.Iso.Model
{
    public class Iso9660BootRecord
    {
        public string BootSystemIdentifier { get; set; }
        public string BootIdentifier { get; set; }
        public byte[] BootSystemData { get; set; }
    }
}