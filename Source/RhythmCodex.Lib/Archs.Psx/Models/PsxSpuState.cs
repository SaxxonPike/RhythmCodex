using System.Linq;

namespace RhythmCodex.Archs.Psx.Models;

public class PsxSpuState
{
    /* 1F801D80h */
    public readonly ushort[] Gain = [0, 0];

    /* 1F801DA4h */
    public ushort IrqAddress;

    /* 1F801DA6h */
    public ushort TransferAddress;

    /* 1F801DA8h */
    public ushort TransferBuffer;

    /* 1F801DAAh */
    public ushort Control;

    /* 1F801DACh */
    public ushort TransferControl;

    /* 1F801DAEh */
    public ushort Status;

    /* 1F801DB0h */
    public readonly ushort[] CddaGain = [0, 0];

    /* 1F801DB4h */
    public readonly ushort[] ExternalGain = [0, 0];

    /* 1F801DB8h */
    public readonly ushort[] CurrentLevel = [0, 0];

    public ushort NoiseLevel = 1;
    
    public readonly PsxSpuReverbState Reverb = new();

    public readonly PsxSpuVoiceState[] Voices = Enumerable.Range(0, 24)
        .Select(_ => new PsxSpuVoiceState())
        .ToArray();
}