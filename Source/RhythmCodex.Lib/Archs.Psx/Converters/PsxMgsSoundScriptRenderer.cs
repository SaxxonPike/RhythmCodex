using System;
using System.Collections.Generic;
using System.Diagnostics;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Converters;

namespace RhythmCodex.Archs.Psx.Converters;

public class PsxMgsSoundScriptRenderer(
    IVagSplitter vagSplitter)
    : IPsxMgsSoundScriptRenderer
{
    private struct SePlayTbl
    {
        public byte Pri { get; set; }
        public byte Kind { get; set; }
        public byte Character { get; set; }
        public int Addr { get; set; }
        public int Code { get; set; }
    }

    private struct SpuTrackReg
    {
        public ushort vol_l { get; set; }
        public ushort vol_r { get; set; }
        public short vol_fg { get; set; }
        public ushort pitch { get; set; }
        public short pitch_fg { get; set; }
        public uint addr { get; set; }
        public short addr_fg { get; set; }
        public long a_mode { get; set; }
        public ushort ar { get; set; }
        public ushort dr { get; set; }
        public short env1_fg { get; set; }
        public long s_mode { get; set; }
        public ushort sr { get; set; }
        public ushort sl { get; set; }
        public short env2_fg { get; set; }
        public long r_mode { get; set; }
        public ushort rr { get; set; }
        public short env3_fg { get; set; }
    }

    public Sound Render(PsxMgsSoundScript script, List<PsxMgsSoundBankEntryWithData> soundBank, int track)
    {
        var result = new SoundBuilder(2);
        Sample? sample;
        var mptr = 0;
        var sptr = new PsxMgsSoundState();
        var finished = false;
        var keyoffs = 0;
        var keyd = 0;
        var eons = 0;
        var eoffs = 0;
        var mtrack = track;
        var spu_tr_wk = new SpuTrackReg();
        var keyFg = 0;
        byte mdata1;
        byte mdata2;
        byte mdata3;
        byte mdata4;
        var se_playing = new SePlayTbl();

        var spu_ch_tbl = track switch
        {
            0 => 1,
            >= 1 and < 25 => 1 << (track - 1)
        };

        while (!finished)
        {
            sptr.Tmpd += sptr.Tmp;
            var tmpd = sptr.Tmpd;

            if (tmpd >= 256)
            {
                sptr.Tmpd = unchecked((ushort)(tmpd & 0xFF));
                --sptr.Ngc;

                if (sptr.Ngc != 0)
                {
                    keych();
                }
                else if (tx_read())
                {
                    keyoff();
                    continue;
                }

                tempo_ch();
                bendch();
                vol_compute();
            }
            else
            {
                note_cntl();
            }

            if (keyFg != 0)
            {
                keyon();
            }
        }

        return result.ToSound();

        bool tx_read()
        {
            var readFg = 1;
            var loopCount = 0;

            while (readFg != 0)
            {
                loopCount++;
                if (loopCount >= 256)
                    return true;

                var packet = script.Packets[mptr];
                mdata1 = (byte)packet.Command;

                if (mdata1 == 0)
                    return true;

                mdata2 = packet.Data2;
                mdata3 = packet.Data3;
                mdata4 = packet.Data4;
                mptr++;

                if (mdata1 < 0x80)
                {
                    if (sptr.Ngg < 0x64 && mdata4 != 0)
                        keyFg = 1;

                    readFg = 0;
                    sptr.RestFg = 0;
                    note_set();
                    continue;
                }

                switch (packet.Command)
                {
                    case PsxMgsSoundTablePacketType.SetTempo:
                    {
                        sptr.Tmp = mdata2;
                        break;
                    }
                    case PsxMgsSoundTablePacketType.MoveTempo:
                    {
                        int tmp_data;

                        sptr.Tmpc = mdata2;
                        sptr.Tmpm = mdata3;
                        sptr.Tmpw = unchecked((ushort)(sptr.Tmp << 8));

                        tmp_data = sptr.Tmpm - sptr.Tmp;
                        if (tmp_data < 0)
                        {
                            if (tmp_data < -127)
                            {
                                tmp_data = -127;
                            }

                            var tmpAd = -((-tmp_data << 8) / sptr.Tmpc);
                            sptr.Tmpad = unchecked((ushort)tmpAd);
                            if (tmpAd < -0x7F0)
                            {
                                sptr.Tmpad = unchecked((ushort)-0x7F0);
                            }
                        }
                        else
                        {
                            if (tmp_data > 127)
                            {
                                tmp_data = 127;
                            }

                            sptr.Tmpad = unchecked((ushort)((tmp_data << 8) / sptr.Tmpc));
                            if (sptr.Tmpad > 0x7F0)
                            {
                                sptr.Tmpad = 0x7F0;
                            }
                        }

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetId:
                    {
                        sptr.Snos = mdata2;
                        keyoff();
                        tone_set(mdata2);
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetVolume:
                    {
                        sptr.Snos = mdata2;
                        keyoff();
                        tone_set(mdata2);
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SvpSet:
                    {
                        sptr.Snos = mdata2;
                        keyoff();
                        tone_set(mdata2);
                        break;
                    }
                    case PsxMgsSoundTablePacketType.ChangeVolume:
                    {
                        sptr.Pvod = unchecked((short)(mdata2 << 8));
                        sptr.Pvoc = 0;
                        break;
                    }
                    case PsxMgsSoundTablePacketType.MoveVolume:
                    {
                        int vol_data;

                        sptr.Pvoc = mdata2;
                        sptr.Pvom = mdata3;
                        vol_data = (mdata3 << 8);
                        vol_data = vol_data - sptr.Pvod;
                        if (vol_data < 0)
                        {
                            var pvoad = -(-vol_data / sptr.Pvoc);
                            sptr.Pvoad = unchecked((short)pvoad);
                            if (sptr.Pvoad < -0x7F0)
                            {
                                sptr.Pvoad = -0x7F0;
                            }
                        }
                        else
                        {
                            sptr.Pvoad = unchecked((short)(vol_data / sptr.Pvoc));
                            if (sptr.Pvoad > 0x7F0)
                            {
                                sptr.Pvoad = 0x7F0;
                            }
                        }

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetAttackDecay:
                    {
                        spu_tr_wk.a_mode = 1;
                        spu_tr_wk.ar = unchecked((ushort)(~mdata2 & 0x7F));
                        spu_tr_wk.dr = unchecked((ushort)(~mdata3 & 0xF));
                        spu_tr_wk.sl = unchecked((ushort)(mdata4 & 0xF));
                        spu_tr_wk.env1_fg = 1;

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetSustainRelease:
                    {
                        spu_tr_wk.s_mode = 3;
                        spu_tr_wk.sr = unchecked((ushort)(~mdata2 & 0x7F));
                        spu_tr_wk.env2_fg = 1;

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetReleaseRate:
                    {
                        var flags = unchecked((ushort)(~mdata2 & 0x1F));
                        spu_tr_wk.r_mode = 3;
                        spu_tr_wk.rr = flags;
                        sptr.Rrd = flags;
                        spu_tr_wk.env3_fg = 1;

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetPanning:
                    {
                        sptr.Panmod = mdata2;
                        sptr.Panf = unchecked((byte)(mdata3 + 20));
                        sptr.Pand = unchecked((short)(sptr.Panf << 8));
                        sptr.Panc = 0;

                        break;
                    }
                    case PsxMgsSoundTablePacketType.MovePanning:
                    {
                        sptr.Panc = mdata2;
                        var pandShift = unchecked((byte)(mdata3 + 0x14));
                        sptr.Panm = unchecked((short)(pandShift << 8));
                        var panData = unchecked((short)(pandShift - sptr.Panf));

                        if (panData < 0)
                        {
                            sptr.Panad = unchecked((short)-((-panData << 8) / mdata2));
                            if (sptr.Panad < -2032)
                            {
                                sptr.Panad = -2032;
                            }
                        }
                        else
                        {
                            sptr.Panad = unchecked((short)((panData << 8) / mdata2));
                            if (sptr.Panad > 2032)
                            {
                                sptr.Panad = 2032;
                            }
                        }

                        break;
                    }
                    case PsxMgsSoundTablePacketType.Transpose:
                    {
                        sptr.Ptps = unchecked((sbyte)mdata2);
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetDetune:
                    {
                        sptr.Tund = unchecked((sbyte)(mdata2 << 2));
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetVibrato:
                    {
                        sptr.Vibhs = mdata2;
                        sptr.Vibcad = mdata3;
                        if (sptr.Vibcad < 64)
                        {
                            if (sptr.Vibcad < 32)
                            {
                                sptr.VibTcOfst = 1;
                                sptr.Vibcad = unchecked((byte)(sptr.Vibcad << 3));
                            }
                            else
                            {
                                sptr.VibTcOfst = 2;
                                sptr.Vibcad = unchecked((byte)(sptr.Vibcad << 2));
                            }
                        }
                        else
                        {
                            if (sptr.Vibcad < 128)
                            {
                                sptr.VibTcOfst = 4;
                                sptr.Vibcad = unchecked((byte)(sptr.Vibcad << 1));
                            }
                            else if (sptr.Vibcad != 255)
                            {
                                sptr.VibTcOfst = 8;
                            }
                            else
                            {
                                sptr.VibTcOfst = 16;
                            }
                        }

                        sptr.Vibd = unchecked((ushort)(mdata4 << 8));
                        sptr.Vibdm = unchecked((ushort)(mdata4 << 8));

                        break;
                    }
                    case PsxMgsSoundTablePacketType.ChangeVibrato:
                    {
                        sptr.Vibcs = mdata2;
                        sptr.Vibad = unchecked((ushort)(sptr.Vibdm / mdata2));
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetRdm:
                    {
                        sptr.Rdms = mdata2;
                        sptr.Rdmd = unchecked((ushort)((mdata3 << 8) + mdata4));
                        sptr.Rdmc = 0;
                        sptr.Rdmo = 0;
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetSwp:
                    {
                        // Ignored.
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetSws:
                    {
                        sptr.Swsk = 0;
                        sptr.Swshc = mdata2;
                        sptr.Swsc = mdata3;
                        sptr.Swss = unchecked((short)(mdata4 << 8));
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetPor:
                    {
                        sptr.Swshc = 0;
                        sptr.Swsc = mdata2;
                        sptr.Swsk = mdata2 == 0 ? (byte)0 : (byte)1;

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetLoop1Start:
                    {
                        sptr.Loop1Addr = mptr;
                        sptr.Loop1Count = 0;
                        sptr.Loop1Freq = 0;
                        sptr.Loop1Volume = 0;

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetLoop1End:
                    {
                        if (mdata2 == 0)
                        {
                            sptr.Loop1Volume = 0;
                            sptr.Loop1Freq = 0;
                        }
                        else
                        {
                            var v1 = unchecked((byte)(sptr.Loop1Count + 1));
                            sptr.Loop1Count = v1;

                            if (v1 != mdata2 || v1 == 0)
                            {
                                sptr.Loop1Volume += unchecked((sbyte)mdata3);
                                sptr.Loop1Freq += unchecked((sbyte)(mdata4 * 8));
                                mptr = sptr.Loop1Addr;
                            }
                            else
                            {
                                sptr.Loop1Volume = 0;
                                sptr.Loop1Freq = 0;
                            }
                        }

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetLoop2Start:
                    {
                        sptr.Loop2Addr = mptr;
                        sptr.Loop2Count = 0;
                        sptr.Loop2Freq = 0;
                        sptr.Loop2Volume = 0;

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetLoop2End:
                    {
                        var cnt = unchecked((byte)(sptr.Loop2Count + 1));
                        sptr.Loop2Count = cnt;

                        if (cnt != mdata2 || cnt == 0)
                        {
                            sptr.Loop2Volume += unchecked((sbyte)mdata3);
                            sptr.Loop2Freq += unchecked((short)(8 * unchecked((sbyte)mdata4)));
                            mptr = sptr.Loop2Addr;
                        }

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetLoop3Start:
                    {
                        sptr.Loop3Addr = mptr;
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetLoop3End:
                    {
                        if (sptr.Loop3Addr != 0)
                        {
                            mptr = sptr.Loop3Addr;
                        }
                        else
                        {
                            keyoffs |= keyd;
                            return false;
                        }

                        break;
                    }
                    case PsxMgsSoundTablePacketType.StartKakko:
                    {
                        sptr.Kak1Pointer = mptr;
                        sptr.KakFg = 0;
                        break;
                    }
                    case PsxMgsSoundTablePacketType.EndKakko:
                    {
                        switch (sptr.KakFg)
                        {
                            case 0:
                                sptr.KakFg++;
                                break;

                            case 1:
                                sptr.KakFg++;
                                sptr.Kak2Pointer = mptr;
                                mptr = sptr.Kak1Pointer;
                                break;

                            case 2:
                                sptr.KakFg--;
                                mptr = sptr.Kak2Pointer;
                                break;
                        }

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetUse:
                    {
                        // Ignored.
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetRest:
                    {
                        sptr.RestFg = 1;
                        keyoff();
                        sptr.Ngs = mdata2;
                        sptr.Ngg = 0;
                        sptr.Vol = 0;
                        sptr.Ngc = sptr.Ngs;
                        sptr.Ngo = 0;

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetTie:
                    {
                        sptr.RestFg = 1;
                        sptr.Ngs = mdata2;
                        sptr.Ngg = mdata3;
                        sptr.Ngc = sptr.Ngs;

                        var temp1 = sptr.Ngg * sptr.Ngc / 100;
                        if (temp1 == 0)
                            temp1 = 1;

                        sptr.Ngo = unchecked((byte)temp1);

                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetEcho1:
                    {
                        // Ignored.
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetEcho2:
                    {
                        // Ignored.
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetEOn:
                    {
                        eons |= spu_ch_tbl << 1;
                        break;
                    }
                    case PsxMgsSoundTablePacketType.SetEOff:
                    {
                        eoffs |= spu_ch_tbl << 1;
                        break;
                    }
                    case PsxMgsSoundTablePacketType.EndBlock:
                    {
                        keyoffs |= keyd;
                        break;
                    }
                    default:
                    {
                        Debug.WriteLine($"Not implemented MGS packet command: {packet.Command}");
                        break;
                    }
                }

                if (mdata1 is 0xF2 or 0xF3 or 0xFF)
                {
                    readFg = 0;
                }

                if (mdata1 == 0xFF)
                    return true;

                continue;
            }

            return false;
        }

        void note_set()
        {
            var x = 0;

            sptr.Ngs = mdata2;
            sptr.Ngg = mdata3;
            sptr.Vol = unchecked((byte)(mdata4 & 0x7F));
            note_compute();
            sptr.Ngc = sptr.Ngs;
            x = sptr.Ngg * sptr.Ngc / 100;

            if (x == 0)
                x = 1;

            sptr.Ngo = unchecked((byte)x);
        }

        void note_compute()
        {
            int x;
            int swp_ex;
            PsxMgsSoundState pSound;

            if (mdata1 >= 0x48)
            {
                drum_set(mdata1);
                x = 0x24;
            }
            else
            {
                x = mdata1;
            }

            x += sptr.Ptps;
            x = (x << 8) + sptr.Tund;
            x = x + sptr.Loop1Freq + sptr.Loop2Freq;

            while (x >= 0x6000)
            {
                x -= 0x6000;
            }

            swp_ex = sptr.Swpd;

            pSound = sptr;
            pSound.Vibcc = 0;
            pSound.Vibhc = 0;
            pSound.Swpd = unchecked((ushort)x);

            sptr.VibTmpCnt = 0;
            sptr.VibTblCnt = 0;

            pSound = sptr;
            pSound.Trehc = 0;
            pSound.Trec = 0;
            pSound.Vibd = 0;

            spu_tr_wk.rr = sptr.Rrd;
            spu_tr_wk.env3_fg = 1;

            sptr.Swpc = sptr.Swsc;

            if (sptr.Swpc != 0)
            {
                sptr.Swphc = sptr.Swshc;

                if (sptr.Swsk == 0)
                {
                    x = sptr.Swpd;

                    if (sptr.Swss >= 0x7F01)
                    {
                        var swpdVal = sptr.Swpd + (0x10000 - (sptr.Swss & 0xFFFF));
                        sptr.Swpd = unchecked((ushort)swpdVal);
                    }
                    else
                    {
                        var swpdVal = sptr.Swpd - sptr.Swss;
                        sptr.Swpd = unchecked((ushort)swpdVal);
                    }

                    swpadset(x);
                }
                else
                {
                    sptr.Swpm = sptr.Swpd;
                    sptr.Swpd = unchecked((ushort)swp_ex);
                }
            }

            freq_set(sptr.Swpd);
        }

        void drum_set(int n)
        {
            const int wavs = 0x4F;
            var addend = wavs + 0xB8;
            n += addend;
            tone_set(n);
        }

        void freq_set(int note_tune)
        {
        }

        void swpadset(int xfreq)
        {
        }

        void note_cntl()
        {
            int rdm_data;
            int fset_fg;
            ushort depth;
            int frq_data;

            if (sptr.Vol != 0 && sptr.Tred != 0 &&
                sptr.Trehs == sptr.Trehc)
            {
                sptr.Trec += unchecked((sbyte)((sptr.Trecad * (char)sptr.Tmpd) >> 8));

                if (sptr.Trec < 0)
                {
                    depth = unchecked((ushort)(sptr.Tred * -sptr.Trec));
                }
                else if (sptr.Trec == 0)
                {
                    depth = 1;
                }
                else
                {
                    depth = unchecked((ushort)(sptr.Tred * sptr.Trec));
                }

                volxset(depth >> 8);
            }

            fset_fg = 0;
            frq_data = sptr.Swpd;

            if (sptr.Swpc != 0 && sptr.Swphc == 0)
            {
                fset_fg = 1;

                if (sptr.Swsk == 0)
                {
                    var swpdVal = sptr.Swpd + sptr.Swpad;
                    sptr.Swpd = unchecked((ushort)swpdVal);
                }
                else
                {
                    por_compute();
                }

                frq_data = sptr.Swpd;
            }

            if (sptr.Vibd != 0 && sptr.Vibhs == sptr.Vibhc)
            {
                sptr.VibTmpCnt += sptr.Vibcad;
                if (sptr.VibTmpCnt >= 256)
                {
                    sptr.VibTmpCnt &= 0xFF;
                    frq_data += vib_compute();
                    fset_fg = 1;
                }
            }

            rdm_data = random();

            if (rdm_data != 0)
            {
                fset_fg = 1;
                frq_data += rdm_data;
            }

            if (fset_fg != 0)
            {
                freq_set(frq_data);
            }
        }

        void volxset(int depth)
        {
            int vol_data;
            int pvod_w;

            vol_data = sptr.Vol;
            vol_data -= depth;
            vol_data += sptr.Loop1Volume;
            vol_data += sptr.Loop2Volume;
            if (vol_data < 0)
            {
                vol_data = 0;
            }
            else if (vol_data >= 128)
            {
                vol_data = 127;
            }
            pvod_w = (sptr.Pvod >> 8) & 0xFF;
            vol_set(((pvod_w * vol_data) >> 8) & 0xFF);
        }

        void vol_set(int vol_data)
        {
            int pan;

            if ((mtrack < 13) ||
                (se_playing.Kind == 0))
            {
                if (vol_data >= sptr.DecVol)
                {
                    vol_data -= sptr.DecVol;
                }
                else
                {
                    vol_data = 0;
                }

                pan = sptr.Pand >> 8;

                if (pan > 40)
                {
                    pan = 40;
                }

                if (mtrack < 13)
                {
                    spu_tr_wk.vol_r = (vol_data * pant[pan] * sng_master_vol) >> 16;
                    spu_tr_wk.vol_l = (vol_data * pant[40 - pan] * sng_master_vol) >> 16;
                    spu_tr_wk.vol_fg = 1;
                }
                else
                {
                    spu_tr_wk.vol_r = vol_data * pant[pan];
                    spu_tr_wk.vol_l = vol_data * pant[40 - pan];
                    spu_tr_wk.vol_fg = 1;
                }
            }
            else
            {
                if (vol_data >= sptr.dec_vol)
                {
                    vol_data -= sptr.dec_vol;
                }
                else
                {
                    vol_data = 0;
                }

                pan = se_pan[mtrack - 13];
                vol_data = (vol_data * se_vol[mtrack - 13]) >> 16;

                spu_tr_wk.vol_r = vol_data * se_pant[pan];
                spu_tr_wk.vol_l = vol_data * se_pant[64 - pan];
                spu_tr_wk.vol_fg = 1;
            }
        }

        int random()
        {
            int  frq_dt = 0;
            int temp2;

            if (sptr.Rdms)
            {
                sptr.Rdmc += sptr.Rdms;
                if (sptr.Rdmc > 256)
                {
                    sptr.Rdmc &= 255;
                    sptr.Rdmo++;
                    sptr.Rdmo &= 0x7F;
                    temp2 = rdm_tbl[sptr.Rdmo];
                    frq_dt = rdm_tbl[sptr.Rdmo + 1] << 8;
                    frq_dt += temp2;
                    frq_dt &= sptr.Rdmd;
                }
            }
            return frq_dt;
        }

        int vib_compute()
        {
            int tmp;
            int          tbl_data;
             int vib_data;

            sptr->vib_tbl_cnt += sptr.VibTcOfst;
            sptr->vib_tbl_cnt &= 0x3Fu;
            tbl_data = VIBX_TBL[sptr->vib_tbl_cnt & 0x1F];

            tmp = sptr.Vibd;
            if (0x7FFF >= tmp)
            {
                vib_data = ((tmp >> 7) & 0xFE);
                vib_data = (vib_data * tbl_data) >> 8;
            }
            else
            {
                vib_data = ((tmp >> 8) & 0x7F) + 2;
                vib_data = (vib_data * tbl_data) >> 1;
            }

            if (sptr.VibTblCnt >= 32u)
            {
                vib_data = -vib_data;
            }

            return vib_data;
        }

        void por_compute()
        {
            int          por_freq;
             int pfreq_h;
             int pfreq_l;

            por_freq = sptr.Swpm - sptr.Swpd;
            if (por_freq < 0)
            {
                por_freq = -por_freq;
                pfreq_l = por_freq & 0xFF;
                pfreq_h = por_freq >> 8;
                pfreq_l = (pfreq_l * sptr.Swsc) >> 8;
                pfreq_h *= sptr.Swsc;
                por_freq = pfreq_h + pfreq_l;

                if (por_freq == 0)
                {
                    por_freq = 1;
                }
                por_freq = -por_freq;
            }
            else if (por_freq == 0)
            {
                sptr.Swpc = 0;
            }
            else
            {
                pfreq_l = por_freq & 0xFF;
                pfreq_h = por_freq >> 8;
                pfreq_l = (pfreq_l * sptr.Swsc) >> 8;
                pfreq_h *= sptr.Swsc;
                por_freq = pfreq_h + pfreq_l;

                if (por_freq == 0)
                {
                    por_freq = 1;
                }
            }

            sptr.Swpd += por_freq;
        }

        void vol_compute()
        {
        }

        void bendch()
        {
        }

        void tempo_ch()
        {
        }

        void keych()
        {
        }

        void keyon()
        {
        }

        void keyoff()
        {
            keyoffs |= keyd;
        }

        void tone_set(int val)
        {
        }
    }
}