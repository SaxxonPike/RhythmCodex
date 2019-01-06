using System.IO;

namespace RhythmCodex.Xact.Model
{
    public struct WaveBankEntry
    {
        public int Value;

        private int Flags => Value & 0xF;
        private int Duration => (Value >> 4);

        public WaveBankMiniWaveFormat Format;
        public WaveBankRegion PlayRegion;
        public WaveBankSampleRegion LoopRegion;

        public static WaveBankEntry Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var result = new WaveBankEntry
            {
                Value = reader.ReadInt32(),
                Format = WaveBankMiniWaveFormat.Read(source),
                PlayRegion = WaveBankRegion.Read(source),
                LoopRegion = WaveBankSampleRegion.Read(source)
            };
            return result;
        }
    }
}