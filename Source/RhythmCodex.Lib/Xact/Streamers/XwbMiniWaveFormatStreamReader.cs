using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XwbMiniWaveFormatStreamReader : IXwbMiniWaveFormatStreamReader
    {
        public WaveBankMiniWaveFormat Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var result = new WaveBankMiniWaveFormat {Value = reader.ReadInt32()};
            return result;
        }
    }
}