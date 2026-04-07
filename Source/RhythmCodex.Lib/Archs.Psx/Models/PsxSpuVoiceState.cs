namespace RhythmCodex.Archs.Psx.Models;

public class PsxSpuVoiceState
{
    /* 1F801C00h+N*10h */
    public readonly ushort[] Gain = [0, 0];

    /* 1F801C04h+N*10h */
    public ushort Rate;

    /* 1F801C06h+N*10h */
    public int StartAddress;

    /* 1F801C08h+N*10h */
    public ushort AdsrMode0;

    /* 1F801C0Ah+N*10h */
    public ushort AdsrMode1;

    /* 1F801C0Ch+N*10h */
    public ushort AdsrLevel;

    /* 1F801C0Eh+N*10h */
    public int RepeatAddress;

    /* 1F801D88h */
    public bool KeyOn;

    /* 1F801D8Ch */
    public bool KeyOff;

    /* 1F801D90h */
    public bool PitchMod;

    /* 1F801D94h */
    public bool NoiseMode;

    /* 1F801D98h */
    public bool ReverbOn;

    /* 1F801D9Ch */
    public bool EndX;

    public readonly byte[] Block = new byte[16];

    public readonly byte[] LastBlock = new byte[16];
}