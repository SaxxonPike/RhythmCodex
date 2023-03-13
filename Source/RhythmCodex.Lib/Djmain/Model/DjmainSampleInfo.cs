using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public class DjmainSampleInfo : IDjmainSampleInfo
{
    /// <inheritdoc />
    public byte Channel { get; set; }
    /// <inheritdoc />
    public byte Flags { get; set; }
    /// <inheritdoc />
    public ushort Frequency { get; set; }
    /// <inheritdoc />
    public uint Offset { get; set; }
    /// <inheritdoc />
    public byte Panning { get; set; }
    /// <inheritdoc />
    public byte ReverbVolume { get; set; }
    /// <inheritdoc />
    public byte SampleType { get; set; }
    /// <inheritdoc />
    public byte Volume { get; set; }
        
    public override string ToString() => Json.Serialize(this);
}