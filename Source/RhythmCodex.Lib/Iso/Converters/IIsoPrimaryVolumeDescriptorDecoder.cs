using System;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters;

public interface IIsoPrimaryVolumeDescriptorDecoder
{
    IsoVolume Decode(ReadOnlySpan<byte> data);
}