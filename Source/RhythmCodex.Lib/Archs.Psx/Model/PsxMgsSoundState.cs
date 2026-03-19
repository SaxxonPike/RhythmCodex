namespace RhythmCodex.Archs.Psx.Model;

public class PsxMgsSoundState
{
    //
    // ref: struct SOUND_W
    // https://github.com/FoxdieTeam/mgs_reversing/blob/master/source/sd/sd_incl.h
    //

    public int MPointer { get; set; }
    public byte Ngc { get; set; } = 1;
    public byte Ngo { get; set; }
    public byte Ngs { get; set; }
    public byte Ngg { get; set; }
    public byte Loop1Count { get; set; }
    public byte Loop2Count { get; set; }
    public short Loop1Volume { get; set; }
    public short Loop2Volume { get; set; }
    public short Loop1Freq { get; set; }
    public short Loop2Freq { get; set; }
    public int Loop1Addr { get; set; }
    public int Loop2Addr { get; set; }
    public int Loop3Addr { get; set; }
    public byte KakFg { get; set; }
    public int Kak1Pointer { get; set; }
    public int Kak2Pointer { get; set; }
    public byte Pvoc { get; set; }
    public short Pvod { get; set; } = 127;
    public short Pvoad { get; set; }
    public ushort Pvom { get; set; }
    public byte Vol { get; set; } = 127;
    public byte Panc { get; set; }
    public short Pand { get; set; } = 2560;
    public short Panad { get; set; }
    public short Panm { get; set; }
    public byte Panf { get; set; } = 10;
    public byte Panoff { get; set; }
    public byte Panmod { get; set; }
    public byte Swpc { get; set; }
    public byte Swphc { get; set; }
    public ushort Swpd { get; set; }
    public short Swpad { get; set; }
    public ushort Swpm { get; set; }
    public byte Swsc { get; set; }
    public byte Swshc { get; set; }
    public byte Swsk { get; set; }
    public short Swss { get; set; }
    public byte Vibhc { get; set; }
    public ushort VibTmpCnt { get; set; }
    public byte VibTblCnt { get; set; }
    public byte VibTcOfst { get; set; }
    public byte Vibcc { get; set; }
    public ushort Vibd { get; set; }
    public ushort Vibdm { get; set; }
    public byte Vibhs { get; set; }
    public byte Vibcs { get; set; }
    public byte Vibcad { get; set; }
    public ushort Vibad { get; set; }
    public ushort Rdmc { get; set; }
    public ushort Rdmo { get; set; }
    public byte Rdms { get; set; }
    public ushort Rdmd { get; set; }
    public sbyte Trec { get; set; }
    public byte Trehc { get; set; }
    public byte Tred { get; set; }
    public byte Trecad { get; set; }
    public byte Trehs { get; set; }
    public ushort Snos { get; set; }
    public short Ptps { get; set; }
    public uint DecVol { get; set; }
    public short Tund { get; set; }
    public ushort Tmpd { get; set; } = 1;
    public byte Tmp { get; set; } = 255;
    public ushort Tmpad { get; set; }
    public byte Tmpc { get; set; }
    public ushort Tmpw { get; set; }
    public byte Tmpm { get; set; }
    public ushort RestFg { get; set; }
    public byte Macro { get; set; }
    public byte Micro { get; set; }
    public ushort Rrd { get; set; }
}