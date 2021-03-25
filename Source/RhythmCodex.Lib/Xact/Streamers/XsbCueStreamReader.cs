using System.Collections.Generic;
using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XsbCueStreamReader : IXsbCueStreamReader
    {
        public IEnumerable<XsbCue> ReadSimple(Stream stream, int count)
        {
            var result = new XsbCue[count];
            var reader = new BinaryReader(stream);

            for (var i = 0; i < count; i++)
            {
                result[i] = new XsbCue
                {
                    Flags = reader.ReadByte(),
                    Offset = reader.ReadInt32()
                };
            }

            return result;
        }

        public IEnumerable<XsbCue> ReadComplex(Stream stream, int count)
        {
            return new XsbCue[count];
        }
    }
}