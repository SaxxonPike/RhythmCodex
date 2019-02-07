using System;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    [Service]
    public class IsoPrimaryVolumeDescriptorDecoder : IIsoPrimaryVolumeDescriptorDecoder
    {
        public Iso9660Volume Decode(ReadOnlySpan<byte> data)
        {
            return new Iso9660Volume
            {
                SystemIdentifier = Encodings.CP437.GetString(data.Slice(8, 32)),
                VolumeIdentifier = Encodings.CP437.GetString(data.Slice(40, 32)),
                SpaceSize = Bitter.ToInt32(data.Slice(80, 8)),
                SetSize = Bitter.ToInt32(data.Slice(120, 4)),
                SequenceNumber = Bitter.ToInt32(data.Slice(124, 4)),
                LogicalBlockSize = Bitter.ToInt32(data.Slice(128, 4)),
                PathTableSize = Bitter.ToInt32(data.Slice(132, 8)),
                TypeLPathTableLocation = Bitter.ToInt32(data.Slice(140, 4)),
                OptionalTypeLPathTableLocation = Bitter.ToInt32(data.Slice(144, 4)),
                TypeMPathTableLocation = Bitter.ToInt32(data.Slice(148, 4)),
                OptionalTypeMPathTableLocation = Bitter.ToInt32(data.Slice(152, 4)),
                // directory record at 156 for 34 bytes
                VolumeSetIdentifier = Encodings.CP437.GetString(data.Slice(190, 128)),
                PublisherIdentifier = Encodings.CP437.GetString(data.Slice(318, 128)),
                DataPreparerIdentifier = Encodings.CP437.GetString(data.Slice(446, 128)),
                ApplicationIdentifier = Encodings.CP437.GetString(data.Slice(574, 128)),
                CopyrightFileIdentifier = Encodings.CP437.GetString(data.Slice(702, 38)),
                AbstractFileIdentifier = Encodings.CP437.GetString(data.Slice(740, 36)),
                BibliographicFileIdentifier = Encodings.CP437.GetString(data.Slice(776, 37)),
                ApplicationData = data.Slice(883, 512).ToArray()
            };
        }
    }
}