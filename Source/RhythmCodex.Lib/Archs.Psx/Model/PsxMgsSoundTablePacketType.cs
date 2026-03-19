namespace RhythmCodex.Archs.Psx.Model;

public enum PsxMgsSoundTablePacketType
{
    //
    // ref: https://github.com/FoxdieTeam/mgs_reversing/blob/master/source/sd/sd_sub1.c
    //

    SetTempo = 0x50,
    MoveTempo = 0x51,
    SetId = 0x52,
    SetVolume = 0x53,
    SvpSet = 0x54,
    ChangeVolume = 0x55,
    MoveVolume = 0x56,
    SetAttackDecay = 0x57,
    SetSustainRelease = 0x58,
    SetReleaseRate = 0x59,
    SetPanning = 0x5D,
    MovePanning = 0x5E,
    Transpose = 0x5F,
    SetDetune = 0x60,
    SetVibrato = 0x61,
    ChangeVibrato = 0x62,
    SetRandom = 0x63,
    SetSwp = 0x64,
    SetSws = 0x65,
    SetPor = 0x66,
    SetLoop1Start = 0x67,
    SetLoop1End = 0x68,
    SetLoop2Start = 0x69,
    SetLoop2End = 0x6A,
    SetLoop3Start = 0x6B,
    SetLoop3End = 0x6C,
    StartKakko = 0x6D,
    EndKakko = 0x6E,
    SetUse = 0x71,
    SetRest = 0x72,
    SetTie = 0x73,
    SetEcho1 = 0x74,
    SetEcho2 = 0x75,
    SetEOn = 0x76,
    SetEOff = 0x77,
    EndBlock = 0x7F
}