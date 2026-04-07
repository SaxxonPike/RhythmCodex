namespace RhythmCodex.Archs.Psx.Models;

public class PsxSpuReverbState
{
    /* 1F801D84h */
    public readonly ushort[] Gain = [0, 0];

    /* 1F801DA2h */
    public ushort WorkAddress;

    /* 1F801DC0h */
    public ushort FbSrcA;

    /* 1F801DC2h */
    public ushort FbSrcB;

    /* 1F801DC4h */
    public short IirAlpha;

    /* 1F801DC6h */
    public short AccCoefA;

    /* 1F801DC8h */
    public short AccCoefB;

    /* 1F801DCAh */
    public short AccCoefC;

    /* 1F801DCCh */
    public short AccCoefD;

    /* 1F801DCEh */
    public short IirCoef;

    /* 1F801DD0h */
    public short FbAlpha;

    /* 1F801DD2h */
    public short FbX;

    /* 1F801DD4h */
    public readonly ushort[] IirDestA = [0, 0];

    /* 1F801DD8h */
    public readonly ushort[] AccSrcA = [0, 0];

    /* 1F801DDCh */
    public readonly ushort[] AccSrcB = [0, 0];

    /* 1F801DE0h */
    public readonly ushort[] IirSrcA = [0, 0];

    /* 1F801DE4h */
    public readonly ushort[] IirDestB = [0, 0];

    /* 1F801DE8h */
    public readonly ushort[] AccSrcC = [0, 0];

    /* 1F801DECh */
    public readonly ushort[] AccSrcD = [0, 0];

    /* 1F801DF0h */
    public readonly ushort[] IirSrcB = [0, 0];

    /* 1F801DF4h */
    public readonly ushort[] MixDestA = [0, 0];

    /* 1F801DF8h */
    public readonly ushort[] MixDestB = [0, 0];

    /* 1F801DFCh */
    public readonly short[] InCoef = [0, 0];
}