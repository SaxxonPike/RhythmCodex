using System.IO;

namespace RhythmCodex.Xact.Model
{
    public struct WaveBankRegion
    {
        public int Offset;
        public int Length;

        public static WaveBankRegion Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var result = new WaveBankRegion {Offset = reader.ReadInt32(), Length = reader.ReadInt32()};
            return result;
        }
    }
}