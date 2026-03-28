namespace RhythmCodex.Archs.Psx.Model;

public enum PsxMgsSoundTablePacketType
{
    //
    // ref: https://github.com/FoxdieTeam/mgs_reversing/blob/master/source/sd/sd_sub1.c
    //

    SetTimeResolution = 0xD0,
    AutomateTimeResolution = 0xD1,
    SetSoundBankIndex = 0xD2,
    SetSoundBankVolume = 0xD3,
    SetSoundBankPanning = 0xD4,
    SetVolumeScale = 0xD5,
    AutomateVolume = 0xD6,
    SetAttackDecay = 0xD7,
    SetSustainRate = 0xD8,
    SetReleaseRate = 0xD9,
    SetPanning = 0xDD,
    AutomatePanning = 0xDE,
    SetCoarseTune = 0xDF,
    SetFineTune = 0xE0,
    SetVibrato = 0xE1,
    ChangeVibrato = 0xE2,
    SetRdm = 0xE3,
    SetPortamentoTime = 0xE4,
    SetSws = 0xE5,
    SetPor = 0xE6,
    SetLoop1Start = 0xE7,
    SetLoop1End = 0xE8,
    SetLoop2Start = 0xE9,
    SetLoop2End = 0xEA,
    SetLoop3Start = 0xEB,
    SetLoop3End = 0xEC,
    StartKakko = 0xED,
    EndKakko = 0xEE,
    SetUse = 0xF1,
    NoteOffAndDelay = 0xF2,
    Delay = 0xF3,
    SetEcho1 = 0xF4,
    SetEcho2 = 0xF5,
    SetEOn = 0xF6,
    SetEOff = 0xF7,
    End = 0xFF
}