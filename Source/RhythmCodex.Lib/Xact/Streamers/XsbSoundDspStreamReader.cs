using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XsbSoundDspStreamReader : IXsbSoundDspStreamReader
    {
        public XsbSoundDsp Read(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var result = new XsbSoundDsp {ExtraData = reader.ReadBytes(7)};
            return result;
        }
    }
}