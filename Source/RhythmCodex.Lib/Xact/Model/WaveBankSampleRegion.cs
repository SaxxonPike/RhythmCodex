using System.IO;

namespace RhythmCodex.Xact.Model
{
    public struct WaveBankSampleRegion
    {
        public int StartSample;
        public int TotalSamples;

        public static WaveBankSampleRegion Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var result = new WaveBankSampleRegion
            {
                StartSample = reader.ReadInt32(),
                TotalSamples = reader.ReadInt32()
            };
            return result;
        }
    }
}