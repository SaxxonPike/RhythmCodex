using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Model;
using RhythmCodex.Iso.Streamers;

namespace RhythmCodex.Iso.Converters
{
    [Service]
    public class IsoPathTableDecoder : IIsoPathTableDecoder
    {
        private readonly ICdSectorStreamFactory _cdSectorStreamFactory;

        public IsoPathTableDecoder(ICdSectorStreamFactory cdSectorStreamFactory)
        {
            _cdSectorStreamFactory = cdSectorStreamFactory;
        }

        public IList<Iso9660PathTableEntry> Decode(IEnumerable<ICdSector> sectors)
        {
            return DecodeInternal(sectors).ToList();
        }

        private IEnumerable<Iso9660PathTableEntry> DecodeInternal(IEnumerable<ICdSector> sectors)
        {
            using (var stream = _cdSectorStreamFactory.Open(sectors))
            {
                var reader = new BinaryReader(stream);
                var length = reader.ReadByte();
                if (length == 0)
                    yield break;
                var eaLength = reader.ReadByte();
                var lba = reader.ReadInt32();
                var parent = reader.ReadUInt16();
                var name = reader.ReadBytes(length);
                if ((length & 1) != 0)
                    reader.ReadByte();
                yield return new Iso9660PathTableEntry
                {
                    LocationOfExtent = lba,
                    DirectoryIdentifier = Encodings.CP437.GetString(name),
                    ExtendedAttributeRecordLength = eaLength,
                    ParentDirectoryNumber = parent
                };
            }
        }
    }
}