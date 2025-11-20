using System;
using System.IO;
using RhythmCodex.FileSystems.Iso.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Iso.Converters;

[Service]
public class IsoDirectoryRecordDecoder(IIsoDateTimeDecoder isoDateTimeDecoder) : IIsoDirectoryRecordDecoder
{
    public IsoDirectoryRecord? Decode(Stream stream, bool recordOnly)
    {
        var reader = new BinaryReader(stream);
        var length = reader.ReadByte();
        if (length == 0)
            return null;
        var eaLength = reader.ReadByte();
        var lba = reader.ReadInt32();
        reader.ReadInt32();
        long dataLength = reader.ReadUInt32();
        reader.ReadInt32();
        var date = reader.ReadBytes(7);
        var flags = reader.ReadByte();
        var unitSize = reader.ReadByte();
        var interleave = reader.ReadByte();
        var volumeSequenceNumber = reader.ReadUInt16();
        reader.ReadInt16();
        var identifierLength = reader.ReadByte();
        var identifier = reader.ReadBytes(identifierLength);
        Memory<byte> extra;
            
        if (recordOnly)
        {
            extra = Memory<byte>.Empty;
        }
        else
        {
            var extraLength = length - identifierLength - 33;
                    
            if ((identifierLength & 1) == 0)
            {
                reader.ReadByte();
                extraLength--;
            }
            extra = reader.ReadBytes(extraLength);
        }

        if (identifier.Length == 1)
        {
            if (identifier[0] == 0x00)
                identifier = [0x2E];
            else if (identifier[0] == 0x01)
                identifier = [0x2E, 0x2E];
        }
                
        return new IsoDirectoryRecord
        {
            ExtendedAttributeRecordLength = eaLength,
            LocationOfExtent = lba,
            DataLength = dataLength,
            RecordingDateTime = isoDateTimeDecoder.Decode(date),
            Flags = (IsoFileFlags) flags,
            UnitSize = unitSize,
            InterleaveGapSize = interleave,
            VolumeSequenceNumber = volumeSequenceNumber,
            Identifier = Encodings.Cp437.GetString(identifier),
            Extra = extra
        };                    
    }
}