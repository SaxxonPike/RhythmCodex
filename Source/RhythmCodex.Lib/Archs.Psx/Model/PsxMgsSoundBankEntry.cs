namespace RhythmCodex.Archs.Psx.Model;

public record PsxMgsSoundBankEntry
{
    //
    // ref: struct WAVE_W
    // https://github.com/FoxdieTeam/mgs_reversing/blob/master/source/sd/sd_incl.h
    //

    public int Offset { get; set; }
    public byte Note { get; set; }
    public byte Tune { get; set; }
    public byte AttackMode { get; set; }
    public byte AttackRate { get; set; }
    public byte DecayRate { get; set; }
    public byte SustainMode { get; set; }
    public byte SustainRate { get; set; }
    public byte SustainLevel { get; set; }
    public byte ReleaseMode { get; set; }
    public byte ReleaseRate { get; set; }
    public byte Pan { get; set; }
    public byte DeclVol { get; set; }
}