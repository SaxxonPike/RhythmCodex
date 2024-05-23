namespace RhythmCodex.Wav.Models;

public interface IWaveFormat
{
    int Channels { get; }
    int SampleRate { get; }
    int ByteRate { get; }
    int BlockAlign { get; }
    int BitsPerSample { get; }
}