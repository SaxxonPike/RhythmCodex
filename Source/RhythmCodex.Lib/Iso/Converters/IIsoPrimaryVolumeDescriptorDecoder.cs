using System;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public interface IIsoPrimaryVolumeDescriptorDecoder
    {
        Iso9660Volume Decode(ReadOnlySpan<byte> data);
    }
}