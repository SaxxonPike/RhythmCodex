using System.IO;
using System.Linq;

namespace RhythmCodex.Xact.Model
{
    public struct WaveBankData
    {
        public int Flags;
        public int EntryCount;
        public string BankName;
        public int EntryMetaDataElementSize;
        public int EntryNameElementSize;
        public int Alignment;
        WaveBankMiniWaveFormat CompactFormat;
        long BuildTime;

        public static WaveBankData Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var result = new WaveBankData
            {
                Flags = reader.ReadInt32(),
                EntryCount = reader.ReadInt32(),
                BankName = new string(reader.ReadChars(XactConstants.WavebankBanknameLength).TakeWhile(c => c != 0)
                    .ToArray()),
                EntryMetaDataElementSize = reader.ReadInt32(),
                EntryNameElementSize = reader.ReadInt32(),
                Alignment = reader.ReadInt32(),
                CompactFormat = new WaveBankMiniWaveFormat {Value = reader.ReadInt32()},
                BuildTime = reader.ReadInt64()
            };
            return result;
        }
    }
}