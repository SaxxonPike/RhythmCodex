using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XwbDataStreamReader : IXwbDataStreamReader
    {
        public XwbData Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var result = new XwbData
            {
                Flags = reader.ReadInt32(),
                EntryCount = reader.ReadInt32(),
                BankName = new string(reader.ReadChars(XwbConstants.WavebankBanknameLength).TakeWhile(c => c != 0)
                    .ToArray()),
                EntryMetaDataElementSize = reader.ReadInt32(),
                EntryNameElementSize = reader.ReadInt32(),
                Alignment = reader.ReadInt32(),
                CompactFormat = new XwbMiniWaveFormat {Value = reader.ReadInt32()},
                BuildTime = reader.ReadInt64()
            };

            return result;
        }
    }
}