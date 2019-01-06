using System.IO;

namespace RhythmCodex.Xact.Model
{
    public struct WaveBankHeader
    {
        public int Signature;
        public int Version;
        public int HeaderVersion;
        public WaveBankRegion[] Segments;

        public static WaveBankHeader Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var result = new WaveBankHeader();
            result.Signature = reader.ReadInt32();
            result.Version = reader.ReadInt32();
            result.HeaderVersion = reader.ReadInt32();
            result.Segments = new WaveBankRegion[(int)WaveBankSegIdx.Count];
            for (var i = 0; i < result.Segments.Length; i++)
                result.Segments[i] = WaveBankRegion.Read(source);
            return result;
        }
    }
}