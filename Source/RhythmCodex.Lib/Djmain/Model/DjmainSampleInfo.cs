using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public record DjmainSampleInfo
{
    /// <summary>
    /// Channel number assigned to the sample. 0x1E is "any channel".
    /// </summary>
    public byte Channel { get; init; }

    /// <summary>
    /// Sample info register flags (K054539 address: 0x201)
    /// </summary>
    public byte Flags { get; init; }

    /// <summary>
    /// Frequency of the sample. Hz = (x * 44100 / 60216)
    /// </summary>
    public ushort Frequency { get; init; }

    /// <summary>
    /// Offset within the chunk where the audio starts.
    /// </summary>
    public uint Offset { get; init; }

    /// <summary>
    /// Left/right panning value, 0x1-0xF
    /// </summary>
    public byte Panning { get; init; }

    /// <summary>
    /// Volume of reverb feedback.
    /// </summary>
    public byte ReverbVolume { get; init; }

    /// <summary>
    /// Sample info register flags (K054539 address: 0x200)
    /// </summary>
    public byte SampleType { get; init; }

    /// <summary>
    /// Volume of the sample.
    /// </summary>
    public byte Volume { get; init; }

    public override string ToString() => Json.Serialize(this);
}