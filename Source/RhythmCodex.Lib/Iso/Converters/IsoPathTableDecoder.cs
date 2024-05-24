using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Cd.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Model;
using RhythmCodex.Iso.Streamers;

namespace RhythmCodex.Iso.Converters;

[Service]
public class IsoPathTableDecoder(IIsoSectorStreamFactory isoSectorStreamFactory) : IIsoPathTableDecoder
{
    public List<IsoPathRecord> Decode(IEnumerable<ICdSector> sectors)
    {
        return DecodeInternal(sectors).ToList();
    }

    private IEnumerable<IsoPathRecord> DecodeInternal(IEnumerable<ICdSector> sectors)
    {
        using var stream = isoSectorStreamFactory.Open(sectors);
        var reader = new BinaryReader(stream);
        while (true)
        {
            var length = reader.ReadByte();
            if (length == 0)
                yield break;
            var eaLength = reader.ReadByte();
            var lba = reader.ReadInt32();
            var parent = reader.ReadUInt16();
            var name = reader.ReadBytes(length);
            if ((length & 1) != 0)
                reader.ReadByte();
                    
            if (name.Length == 1)
            {
                if (name[0] == 0x00)
                    name = [0x2E];
                else if (name[0] == 0x01)
                    name = [0x2E, 0x2E];
            }
                    
            yield return new IsoPathRecord
            {
                LocationOfExtent = lba,
                DirectoryIdentifier = Encodings.CP437.GetString(name),
                ExtendedAttributeRecordLength = eaLength,
                ParentDirectoryNumber = parent
            };                    
        }
    }
}