using System;
using RhythmCodex.FileSystems.Iso.Model;

namespace RhythmCodex.FileSystems.Iso.Converters;

public interface IIsoPrimaryVolumeDescriptorDecoder
{
    IsoVolume Decode(ReadOnlySpan<byte> data);
}