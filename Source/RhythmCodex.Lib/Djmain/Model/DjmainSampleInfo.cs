using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model;

[Model]
public record DjmainSampleInfo
{
    /// <summary>
    /// Channel number assigned to the sample. 0x1E is "any channel".
    /// </summary>
    public byte Channel { get; set; }

    /// <summary>
    /// Sample info register flags (K054539 address: 0x201)
    /// </summary>
    public byte Flags { get; set; }

    /// <summary>
    /// Frequency of the sample. Hz = (x * 44100 / 60216)
    /// </summary>
    public ushort Frequency { get; set; }

    /// <summary>
    /// Offset within the chunk where the audio starts.
    /// </summary>
    public uint Offset { get; set; }

    /// <summary>
    /// Left/right panning value, 0x1-0xF
    /// </summary>
    public byte Panning { get; set; }

    /// <summary>
    /// Volume of reverb feedback.
    /// </summary>
    public byte ReverbVolume { get; set; }

    /// <summary>
    /// Sample info register flags (K054539 address: 0x200)
    /// </summary>
    public byte SampleType { get; set; }

    /// <summary>
    /// Volume of the sample.
    /// </summary>
    public byte Volume { get; set; }

    public override string ToString() => Json.Serialize(this);
}