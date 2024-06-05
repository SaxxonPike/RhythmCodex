namespace RhythmCodex.Vag.Models;

public class VagConfig
{
    /// <summary>
    /// Size of each channel block, in bytes.
    /// Not used for mono streams.
    /// Changing this setting has no effect after a stream is created.
    /// </summary>
    public int Interleave { get; set; }

    /// <summary>
    /// Number of channels in the stream.
    /// For multichannel streams, be sure to set <see cref="Interleave"/>.
    /// Changing this setting has no effect after a stream is created.
    /// </summary>
    public int Channels { get; set; } = 1;

    /// <summary>
    /// Cofficients to be used by the codec.
    /// Leave this null to use the defaults.
    /// Use dimensions of int[5][2].
    /// Changing this setting has no effect after a stream is created.
    /// </summary>
    public int[][]? Coefficients { get; set; }

    /// <summary>
    /// If true, decoding will end on sixteen consecutive zero bytes.
    /// Not used when encoding.
    /// If decoding tends to run well past the end, set this to true.
    /// </summary>
    public bool StopDecodingOnBlankLine { get; set; }

    /// <summary>
    /// If true, the end-of-stream bit determines the end of the stream.
    /// Not used when encoding.
    /// If decoding ends prematurely on known good data, set this to false.
    /// </summary>
    public bool StopDecodingOnEndMarker { get; set; } = true;

    /// <summary>
    /// If true, the start-of-stream bit determines the start of the stream.
    /// Not used when encoding.
    /// If decoding produces a bit of garbage at the start of the data, set this to true.
    /// </summary>
    public bool DoNotDecodeUntilStartMarker { get; set; }

    /// <summary>
    /// Maximum length of the stream.
    /// </summary>
    public long MaximumLength { get; set; } = long.MaxValue;
}