namespace RhythmCodex.Games.Mgs.Models;

public record MgsSdSoundBankEntry
{
    //
    // ref: struct WAVE_W
    // https://github.com/FoxdieTeam/mgs_reversing/blob/master/source/sd/sd_incl.h
    //

    public int Offset { get; init; }
    public byte Note { get; init; }
    public byte Tune { get; init; }
    public byte AttackMode { get; init; }
    public byte AttackRate { get; init; }
    public byte DecayRate { get; init; }
    public byte SustainMode { get; init; }
    public byte SustainRate { get; init; }
    public byte SustainLevel { get; init; }
    public byte ReleaseMode { get; init; }
    public byte ReleaseRate { get; init; }
    public byte Pan { get; init; }
    public byte DeclVol { get; init; }
}