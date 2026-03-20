using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Converters;

namespace RhythmCodex.Archs.Psx.Converters;

[Service]
public class PsxMgsSoundScriptRenderer(
    IVagSplitter vagSplitter)
    : IPsxMgsSoundScriptRenderer
{
    private const int Wavs = 0x4F;
    private const int SpuOn = 1;
    private const int SpuOff = 0;
    private const int SpuCommonMvoll = 1 << 0;
    private const int SpuCommonMvolr = 1 << 1;
    private const int SpuVoiceVoll = 1 << 0;
    private const int SpuVoiceVolr = 1 << 1;
    private const int SpuVoicePitch = 1 << 4;
    private const int SpuVoiceWdsa = 1 << 7;
    private const int SpuVoiceAdsrAmode = 1 << 8;
    private const int SpuVoiceAdsrSmode = 1 << 9;
    private const int SpuVoiceAdsrRmode = 1 << 10;
    private const int SpuVoiceAdsrAr = 1 << 11;
    private const int SpuVoiceAdsrDr = 1 << 12;
    private const int SpuVoiceAdsrSr = 1 << 13;
    private const int SpuVoiceAdsrRr = 1 << 14;
    private const int SpuVoiceAdsrSl = 1 << 15;
    private const int SpuWaveStartPtr = 0;

    private struct SpuVolume
    {
        public int Left;
        public int Right;
    }

    private struct SpuVoiceAttr
    {
        public int Voice;
        public int Mask;
        public SpuVolume Volume;
        public SpuVolume Volmode;
        public SpuVolume Volumex;
        public int Pitch;
        public int Note;
        public int SampleNote;
        public int Envx;
        public int Addr;
        public int LoopAddr;
        public int AMode;
        public int SMode;
        public int RMode;
        public int Ar;
        public int Dr;
        public int Sr;
        public int Rr;
        public int Sl;
        public int Adsr1;
        public int Adsr2;
    }

    private struct SpuCommonAttr
    {
        public int Mask;
        public SpuVolume Mvol;
        public SpuVolume Mvolmode;
        public SpuVolume Mvolx;
        public int Cd;
        public int Ext;
    }

    private struct SoundW
    {
        public int Mpointer { get; set; }
        public int Ngc { get; set; }
        public int Ngo { get; set; }
        public int Ngs { get; set; }
        public int Ngg { get; set; }
        public int Lp1Cnt { get; set; }
        public int Lp2Cnt { get; set; }
        public int Lp1Vol { get; set; }
        public int Lp2Vol { get; set; }
        public int Lp1Freq { get; set; }
        public int Lp2Freq { get; set; }
        public int Lp1Addr { get; set; }
        public int Lp2Addr { get; set; }
        public int Lp3Addr { get; set; }
        public int Kakfg { get; set; }
        public int Kak1Ptr { get; set; }
        public int Kak2Ptr { get; set; }
        public int Pvoc { get; set; }
        public int Pvod { get; set; }
        public int Pvoad { get; set; }
        public int Pvom { get; set; }
        public int Vol { get; set; }
        public int Panc { get; set; }
        public int Pand { get; set; }
        public int Panad { get; set; }
        public int Panm { get; set; }
        public int Panf { get; set; }
        public int Panoff { get; set; }
        public int Panmod { get; set; }
        public int Swpc { get; set; }
        public int Swphc { get; set; }
        public int Swpd { get; set; }
        public int Swpad { get; set; }
        public int Swpm { get; set; }
        public int Swsc { get; set; }
        public int Swshc { get; set; }
        public int Swsk { get; set; }
        public int Swss { get; set; }
        public int Vibhc { get; set; }
        public int VibTmpCnt { get; set; }
        public int VibTblCnt { get; set; }
        public int VibTcOfst { get; set; }
        public int Vibcc { get; set; }
        public int Vibd { get; set; }
        public int Vibdm { get; set; }
        public int Vibhs { get; set; }
        public int Vibcs { get; set; }
        public int Vibcad { get; set; }
        public int Vibad { get; set; }
        public int Rdmc { get; set; }
        public int Rdmo { get; set; }
        public int Rdms { get; set; }
        public int Rdmd { get; set; }
        public int Trec { get; set; }
        public int Trehc { get; set; }
        public int Tred { get; set; }
        public int Trecad { get; set; }
        public int Trehs { get; set; }
        public int Snos { get; set; }
        public int Ptps { get; set; }
        public int DecVol { get; set; }
        public int Tund { get; set; }
        public int Tmpd { get; set; }
        public int Tmp { get; set; }
        public int Tmpad { get; set; }
        public int Tmpc { get; set; }
        public int Tmpw { get; set; }
        public int Tmpm { get; set; }
        public int RestFg { get; set; }
        public int Macro { get; set; }
        public int Micro { get; set; }
        public int Rrd { get; set; }
    }

    private struct WaveW
    {
        public int Addr { get; set; }
        public int SampleNote { get; set; }
        public int SampleTune { get; set; }
        public int AMode { get; set; }
        public int Ar { get; set; }
        public int Dr { get; set; }
        public int SMode { get; set; }
        public int Sr { get; set; }
        public int Sl { get; set; }
        public int RMode { get; set; }
        public int Rr { get; set; }
        public int Pan { get; set; }
        public int DeclVol { get; set; }
    }

    private struct Seplaytbl
    {
        public int Pri { get; set; }
        public int Kind { get; set; }
        public int Character { get; set; }
        public int Addr { get; set; }
        public int Code { get; set; }
    }

    private struct SpuTrackReg
    {
        public int VolL { get; set; }
        public int VolR { get; set; }
        public int VolFg { get; set; }
        public int Pitch { get; set; }
        public int PitchFg { get; set; }
        public int Addr { get; set; }
        public int AddrFg { get; set; }
        public int AMode { get; set; }
        public int Ar { get; set; }
        public int Dr { get; set; }
        public int Env1Fg { get; set; }
        public int SMode { get; set; }
        public int Sr { get; set; }
        public int Sl { get; set; }
        public int Env2Fg { get; set; }
        public int RMode { get; set; }
        public int Rr { get; set; }
        public int Env3Fg { get; set; }
    }

    public Sound Render(PsxMgsSoundScript script, List<PsxMgsSoundBankEntryWithData> soundBank, int track)
    {
        var mdata1 = 0;
        var mdata2 = 0;
        var mdata3 = 0;
        var mdata4 = 0;
        var sptr = new SoundW();
        var spuTrWk = new SpuTrackReg[23];
        var mtrack = track;
        var keyoffs = 0;
        var keyons = 0;
        var eoffs = 0;
        var eons = 0;
        var keyd = 0;
        var songEnd = 0;
        var mptr = 0;
        var sePlaying = new Seplaytbl[8];
        var keyFg = 0;
        var sePan = new int[8];
        var seVol = new int[8];
        const int soundMonoFg = 0;
        var sngMasterVol = new int[13];
        var stopJouchuuSe = 0;
        const int sngKaihiTime = 0;

        //
        // Serialize the tables in the format used by the sound engine.
        //

        var voiceTbl = soundBank.ToDictionary(
            x => x.Index,
            x => new WaveW
            {
                Addr = x.Entry.Offset,
                SampleNote = x.Entry.Note,
                SampleTune = x.Entry.Tune,
                AMode = x.Entry.AttackMode,
                Ar = x.Entry.AttackRate,
                Dr = x.Entry.DecayRate,
                SMode = x.Entry.SustainMode,
                Sr = x.Entry.SustainRate,
                Sl = x.Entry.SustainLevel,
                RMode = x.Entry.ReleaseMode,
                Rr = x.Entry.ReleaseRate,
                Pan = x.Entry.Pan,
                DeclVol = x.Entry.DeclVol
            });

        var scriptData = new byte[script.Packets.Count * 4];

        for (var i = 0; i < script.Packets.Count; i++)
        {
            var packet = script.Packets[i];
            var scriptRecord = scriptData.AsSpan(i * 4, 4);
            scriptRecord[3] = (byte)packet.Command;
            scriptRecord[2] = packet.Data2;
            scriptRecord[1] = packet.Data3;
            scriptRecord[0] = packet.Data4;
        }

        //
        // Fill in some default values.
        //

        sngMasterVol.AsSpan().Fill(0xFFFF);
        seVol.AsSpan().Fill(0xFFFF);

        //
        // Define tables that are used by the sound engine.
        //

        var spuChTbl = new[]
        {
            0x00000001,
            0x00000001, 0x00000002, 0x00000004, 0x00000008,
            0x00000010, 0x00000020, 0x00000040, 0x00000080,
            0x00000100, 0x00000200, 0x00000400, 0x00000800,
            0x00001000, 0x00002000, 0x00004000, 0x00008000,
            0x00010000, 0x00020000, 0x00040000, 0x00080000,
            0x00100000, 0x00200000, 0x00400000, 0x00800000
        };

        var cntlTbl = new List<Action>
        {
            /* 0x00 */ NoCmd,
            /* 0x01 */ NoCmd,
            /* 0x02 */ NoCmd,
            /* 0x03 */ NoCmd,
            /* 0x04 */ NoCmd,
            /* 0x05 */ NoCmd,
            /* 0x06 */ NoCmd,
            /* 0x07 */ NoCmd,
            /* 0x08 */ NoCmd,
            /* 0x09 */ NoCmd,
            /* 0x0a */ NoCmd,
            /* 0x0b */ NoCmd,
            /* 0x0c */ NoCmd,
            /* 0x0d */ NoCmd,
            /* 0x0e */ NoCmd,
            /* 0x0f */ NoCmd,
            /* 0x10 */ NoCmd,
            /* 0x11 */ NoCmd,
            /* 0x12 */ NoCmd,
            /* 0x13 */ NoCmd,
            /* 0x14 */ NoCmd,
            /* 0x15 */ NoCmd,
            /* 0x16 */ NoCmd,
            /* 0x17 */ NoCmd,
            /* 0x18 */ NoCmd,
            /* 0x19 */ NoCmd,
            /* 0x1a */ NoCmd,
            /* 0x1b */ NoCmd,
            /* 0x1c */ NoCmd,
            /* 0x1d */ NoCmd,
            /* 0x1e */ NoCmd,
            /* 0x1f */ NoCmd,
            /* 0x20 */ NoCmd,
            /* 0x21 */ NoCmd,
            /* 0x22 */ NoCmd,
            /* 0x23 */ NoCmd,
            /* 0x24 */ NoCmd,
            /* 0x25 */ NoCmd,
            /* 0x26 */ NoCmd,
            /* 0x27 */ NoCmd,
            /* 0x28 */ NoCmd,
            /* 0x29 */ NoCmd,
            /* 0x2a */ NoCmd,
            /* 0x2b */ NoCmd,
            /* 0x2c */ NoCmd,
            /* 0x2d */ NoCmd,
            /* 0x2e */ NoCmd,
            /* 0x2f */ NoCmd,
            /* 0x30 */ NoCmd,
            /* 0x31 */ NoCmd,
            /* 0x32 */ NoCmd,
            /* 0x33 */ NoCmd,
            /* 0x34 */ NoCmd,
            /* 0x35 */ NoCmd,
            /* 0x36 */ NoCmd,
            /* 0x37 */ NoCmd,
            /* 0x38 */ NoCmd,
            /* 0x39 */ NoCmd,
            /* 0x3a */ NoCmd,
            /* 0x3b */ NoCmd,
            /* 0x3c */ NoCmd,
            /* 0x3d */ NoCmd,
            /* 0x3e */ NoCmd,
            /* 0x3f */ NoCmd,
            /* 0x40 */ NoCmd,
            /* 0x41 */ NoCmd,
            /* 0x42 */ NoCmd,
            /* 0x43 */ NoCmd,
            /* 0x44 */ NoCmd,
            /* 0x45 */ NoCmd,
            /* 0x46 */ NoCmd,
            /* 0x47 */ NoCmd,
            /* 0x48 */ NoCmd,
            /* 0x49 */ NoCmd,
            /* 0x4a */ NoCmd,
            /* 0x4b */ NoCmd,
            /* 0x4c */ NoCmd,
            /* 0x4d */ NoCmd,
            /* 0x4e */ NoCmd,
            /* 0x4f */ NoCmd,
            /* 0x50 */ TempoSet,
            /* 0x51 */ TempoMove,
            /* 0x52 */ SnoSet,
            /* 0x53 */ SvlSet,
            /* 0x54 */ SvpSet,
            /* 0x55 */ VolChg,
            /* 0x56 */ VolMove,
            /* 0x57 */ AdsSet,
            /* 0x58 */ SrsSet,
            /* 0x59 */ RrsSet,
            /* 0x5a */ NoCmd,
            /* 0x5b */ NoCmd,
            /* 0x5c */ NoCmd,
            /* 0x5d */ PanSet,
            /* 0x5e */ PanMove,
            /* 0x5f */ TransSet,
            /* 0x60 */ DetuneSet,
            /* 0x61 */ VibSet,
            /* 0x62 */ VibChange,
            /* 0x63 */ RdmSet,
            /* 0x64 */ SwpSet,
            /* 0x65 */ SwsSet,
            /* 0x66 */ PorSet,
            /* 0x67 */ Lp1Start,
            /* 0x68 */ Lp1End,
            /* 0x69 */ Lp2Start,
            /* 0x6a */ Lp2End,
            /* 0x6b */ L3SSet,
            /* 0x6c */ L3ESet,
            /* 0x6d */ KakkoStart,
            /* 0x6e */ KakkoEnd,
            /* 0x6f */ NoCmd,
            /* 0x70 */ NoCmd,
            /* 0x71 */ UseSet,
            /* 0x72 */ RestSet,
            /* 0x73 */ TieSet,
            /* 0x74 */ EchoSet1,
            /* 0x75 */ EchoSet2,
            /* 0x76 */ EonSet,
            /* 0x77 */ EofSet,
            /* 0x78 */ NoCmd,
            /* 0x79 */ NoCmd,
            /* 0x7a */ NoCmd,
            /* 0x7b */ NoCmd,
            /* 0x7c */ NoCmd,
            /* 0x7d */ NoCmd,
            /* 0x7e */ NoCmd,
            /* 0x7f */ BlockEnd
        };

        int[] rdmTbl =
        [
            159, 60, 178, 82, 175, 69, 199, 137,
            16, 127, 224, 157, 220, 31, 97, 22,
            57, 201, 156, 235, 87, 8, 102, 248,
            90, 36, 191, 14, 62, 21, 75, 219,
            171, 245, 49, 12, 67, 2, 85, 222,
            65, 218, 189, 174, 25, 176, 72, 87,
            186, 163, 54, 11, 249, 223, 23, 168,
            4, 12, 224, 145, 24, 93, 221, 211,
            40, 138, 242, 17, 89, 111, 6, 10,
            52, 42, 121, 172, 94, 167, 131, 198,
            57, 193, 180, 58, 63, 254, 79, 239,
            31, 0, 48, 153, 76, 40, 131, 237,
            138, 47, 44, 102, 63, 214, 108, 183,
            73, 34, 188, 101, 250, 207, 2, 177,
            70, 240, 154, 215, 226, 15, 17, 197,
            116, 246, 122, 44, 143, 251, 25, 106,
            229
        ];

        int[] vibxTbl =
        [
            0, 32, 56, 80, 104, 128, 144, 160,
            176, 192, 208, 224, 232, 240, 240, 248,
            255, 248, 244, 240, 232, 224, 208, 192,
            176, 160, 144, 128, 104, 80, 56, 32
        ];

        int[] pant =
        [
            0, 2, 4, 7, 10, 13, 16, 20, 24, 28, 32, 36, 40, 45,
            50, 55, 60, 65, 70, 75, 80, 84, 88, 92, 96, 100, 104, 107,
            110, 112, 114, 116, 118, 120, 122, 123, 124, 125, 126, 127, 127
        ];

        int[] sePant =
        [
            0, 2, 4, 6, 8, 10, 14, 18, 22, 28, 34, 40, 46,
            52, 58, 64, 70, 76, 82, 88, 94, 100, 106, 112, 118, 124,
            130, 136, 142, 148, 154, 160, 166, 172, 178, 183, 188, 193, 198,
            203, 208, 213, 217, 221, 224, 227, 230, 233, 236, 238, 240, 242,
            244, 246, 248, 249, 250, 251, 252, 253, 254, 254, 255, 255, 255
        ];

        int[] freqTbl =
        [
            0x010B, 0x011B, 0x012C, 0x013E, 0x0151, 0x0165, 0x017A, 0x0191,
            0x01A9, 0x01C2, 0x01DD, 0x01F9, 0x0217, 0x0237, 0x0259, 0x027D,
            0x02A3, 0x02CB, 0x02F5, 0x0322, 0x0352, 0x0385, 0x03BA, 0x03F3,
            0x042F, 0x046F, 0x04B2, 0x04FA, 0x0546, 0x0596, 0x05EB, 0x0645,
            0x06A5, 0x070A, 0x0775, 0x07E6, 0x085F, 0x08DE, 0x0965, 0x09F4,
            0x0A8C, 0x0B2C, 0x0BD6, 0x0C8B, 0x0D4A, 0x0E14, 0x0EEA, 0x0FCD,
            0x10BE, 0x11BD, 0x12CB, 0x13E9, 0x1518, 0x1659, 0x17AD, 0x1916,
            0x1A94, 0x1C28, 0x1DD5, 0x1F9B, 0x217C, 0x237A, 0x2596, 0x27D2,
            0x2A30, 0x2CB2, 0x2F5A, 0x322C, 0x3528, 0x3850, 0x3BAC, 0x3F36,
            0x0021, 0x0023, 0x0026, 0x0028, 0x002A, 0x002D, 0x002F, 0x0032,
            0x0035, 0x0038, 0x003C, 0x003F, 0x0042, 0x0046, 0x004B, 0x004F,
            0x0054, 0x0059, 0x005E, 0x0064, 0x006A, 0x0070, 0x0077, 0x007E,
            0x0085, 0x008D, 0x0096, 0x009F, 0x00A8, 0x00B2, 0x00BD, 0x00C8,
            0x00D4, 0x00E1, 0x00EE, 0x00FC
        ];

        var result = new SoundBuilder(2);

        return result.ToSound();

        // ==================================================================
        //
        // The functions below simulate the Playstation SPU by HLE.
        //
        // ==================================================================

        void SpuSetCommonAttr(ref SpuCommonAttr attr)
        {
        }

        void SpuSetKey(int value, int mask)
        {
        }

        void SpuSetReverbVoice(int value, int mask)
        {
        }

        void SpuSetVoiceAttr(ref SpuVoiceAttr attr)
        {
        }

        // ==================================================================
        //
        // The functions below are ported from the MGS recompilation project:
        // https://github.com/FoxdieTeam/mgs_reversing
        //
        // ==================================================================

        void Spuwr()
        {
            int i;

            if (keyoffs != 0)
            {
                SpuSetKey(SpuOff, keyoffs);
                keyoffs = 0;
            }

            if (eoffs != 0)
            {
                SpuSetReverbVoice(SpuOff, eoffs);
                eoffs = 0;
            }

            for (i = 0; i < 21; i++)
            {
                var attr = new SpuVoiceAttr
                {
                    Mask = 0,
                    Voice = spuChTbl[i + 1]
                };

                if (spuTrWk[i].VolFg != 0)
                {
                    attr.Mask |= SpuVoiceVoll | SpuVoiceVolr;
                    attr.Volume.Left = spuTrWk[i].VolL;
                    attr.Volume.Right = spuTrWk[i].VolR;
                    spuTrWk[i].VolFg = 0;
                }

                if (spuTrWk[i].PitchFg != 0)
                {
                    attr.Mask |= SpuVoicePitch;
                    attr.Pitch = spuTrWk[i].Pitch;
                    spuTrWk[i].PitchFg = 0;
                }

                if (spuTrWk[i].AddrFg != 0)
                {
                    attr.Mask |= SpuVoiceWdsa;
                    attr.Addr = spuTrWk[i].Addr + SpuWaveStartPtr;
                    spuTrWk[i].AddrFg = 0;
                }

                if (spuTrWk[i].Env1Fg != 0)
                {
                    attr.Mask |= SpuVoiceAdsrAmode | SpuVoiceAdsrAr | SpuVoiceAdsrDr;
                    attr.AMode = spuTrWk[i].AMode;
                    attr.Ar = spuTrWk[i].Ar;
                    attr.Dr = spuTrWk[i].Dr;
                    spuTrWk[i].Env1Fg = 0;
                }

                if (spuTrWk[i].Env2Fg != 0)
                {
                    attr.Mask |= SpuVoiceAdsrSmode | SpuVoiceAdsrSr | SpuVoiceAdsrSl;
                    attr.SMode = spuTrWk[i].SMode;
                    attr.Sr = spuTrWk[i].Sr;
                    attr.Sl = spuTrWk[i].Sl;
                    spuTrWk[i].Env2Fg = 0;
                }

                if (spuTrWk[i].Env3Fg != 0)
                {
                    attr.Mask |= SpuVoiceAdsrRmode | SpuVoiceAdsrRr;
                    attr.RMode = spuTrWk[i].RMode;
                    attr.Rr = spuTrWk[i].Rr;
                    spuTrWk[i].Env3Fg = 0;
                }

                if (attr.Mask != 0)
                {
                    SpuSetVoiceAttr(ref attr);
                }
            }

            if (eons != 0)
            {
                SpuSetReverbVoice(SpuOn, eons);
                eons = 0;
            }

            if (keyons != 0)
            {
                SpuSetKey(SpuOn, keyons);
                keyons = 0;
            }
        }

        void SoundOff()
        {
            int i;

            for (i = 0; i < 23; i++)
            {
                spuTrWk[i].Rr = 7;
                spuTrWk[i].Env3Fg = 1;

                var keyNo = spuChTbl[mtrack + 1];
                songEnd |= keyNo;
            }

            keyoffs = 0x7FFFFF;
        }

        void SngOff()
        {
            int i;

            for (i = 0; i < 13; i++)
            {
                spuTrWk[i].Rr = 7;
                spuTrWk[i].Env3Fg = 1;
            }

            songEnd |= 0x1FFF;
            keyoffs |= 0x1FFF;
        }

        void SeOff(int i)
        {
            spuTrWk[i + 13].Env3Fg = 1;
            spuTrWk[i + 13].Rr = 0;
            songEnd |= 1 << (i + 13);
            keyoffs |= 1 << (i + 13);
        }

        void SngPause()
        {
            var cAttr = new SpuCommonAttr
            {
                Mask = SpuCommonMvoll | SpuCommonMvolr
            };

            cAttr.Mvol.Left = 0;
            cAttr.Mvol.Right = 0;
            SpuSetCommonAttr(ref cAttr);
        }

        void SngPauseOff()
        {
            var cAttr = new SpuCommonAttr
            {
                Mask = SpuCommonMvoll | SpuCommonMvolr
            };

            cAttr.Mvol.Left = 0x3FFF;
            cAttr.Mvol.Right = 0x3FFF;
            SpuSetCommonAttr(ref cAttr);
        }

        void Keyon()
        {
            keyons |= keyd;
        }

        void Keyoff()
        {
            keyoffs |= keyd;
        }

        void ToneSet(int n)
        {
            spuTrWk[mtrack].Addr = voiceTbl[n].Addr;
            spuTrWk[mtrack].AddrFg = 1;

            sptr.Macro = voiceTbl[n].SampleNote;
            sptr.Micro = voiceTbl[n].SampleTune;

            spuTrWk[mtrack].AMode = voiceTbl[n].AMode != 0 ? 5 : 1;
            spuTrWk[mtrack].Ar = ~voiceTbl[n].Ar & 0x7F;
            spuTrWk[mtrack].Dr = ~voiceTbl[n].Dr & 0xF;
            spuTrWk[mtrack].Env1Fg = 1;

            spuTrWk[mtrack].SMode = voiceTbl[n].SMode switch
            {
                0 => 3,
                1 => 7,
                2 => 1,
                _ => 5
            };

            spuTrWk[mtrack].Sr = ~voiceTbl[n].Sr & 0x7F;
            spuTrWk[mtrack].Sl = voiceTbl[n].Sl & 0xF;
            spuTrWk[mtrack].Env2Fg = 1;
            spuTrWk[mtrack].RMode = voiceTbl[n].RMode == 0 ? 3 : 7;
            spuTrWk[mtrack].Rr = sptr.Rrd = ~voiceTbl[n].Rr & 0x1F;
            spuTrWk[mtrack].Env3Fg = 1;

            if (sptr.Panmod == 0)
                PanSet2(voiceTbl[n].Pan);

            sptr.DecVol = voiceTbl[n].DeclVol;
        }

        void PanSet2(int x)
        {
            if (sptr.Panoff != 0)
                return;

            sptr.Panf = 2 * x;
            sptr.Pand = x << 9;
        }

        void VolSet(int volData)
        {
            int pan;

            if (mtrack < 13 || sePlaying[mtrack - 13].Kind == 0)
            {
                if (volData >= sptr.DecVol)
                    volData -= sptr.DecVol;
                else
                    volData = 0;

                pan = sptr.Pand >> 8;

                if (pan > 40)
                    pan = 40;

                if (soundMonoFg != 0)
                {
                    pan = 20;
                }

                if (mtrack < 13)
                {
                    spuTrWk[mtrack].VolR = (volData * pant[pan] * sngMasterVol[mtrack]) >> 16;
                    spuTrWk[mtrack].VolL = (volData * pant[40 - pan] * sngMasterVol[mtrack]) >> 16;
                }
                else
                {
                    spuTrWk[mtrack].VolR = volData * pant[pan];
                    spuTrWk[mtrack].VolL = volData * pant[40 - pan];
                }
            }
            else
            {
                if (volData >= sptr.DecVol)
                    volData -= sptr.DecVol;
                else
                    volData = 0;

                pan = sePan[mtrack - 13];
                volData = (volData * seVol[mtrack - 13]) >> 16;

                if (soundMonoFg != 0)
                {
                    pan = 32;
                }

                spuTrWk[mtrack].VolR = volData * sePant[pan];
                spuTrWk[mtrack].VolL = volData * sePant[64 - pan];
            }

            spuTrWk[mtrack].VolFg = 1;
        }

        void FreqSet(int noteTune)
        {
            noteTune += sptr.Micro;
            var temp4 = noteTune;
            var temp3 = (noteTune >> 8) + sptr.Macro;
            temp3 &= 0x7F;
            var freq = freqTbl[temp3 + 1] - freqTbl[temp3];

            if ((freq & 0x8000) != 0)
                freq = 0xC9;

            var temp = freq;
            var temp2 = freq >> 8;
            freq = ((temp * temp4) >> 8) + temp2 * temp4;
            freq += freqTbl[temp3];

            spuTrWk[mtrack].Pitch = freq;
            spuTrWk[mtrack].PitchFg = 1;
        }

        void DrumSet(int n)
        {
            const int addend = Wavs + 0xB8;
            n += addend;
            ToneSet(n);
        }

        int SoundSub()
        {
            keyFg = 0;
            sptr.Tmpd += sptr.Tmp;
            if (mtrack < 13)
            {
                if (sngKaihiTime != 0)
                {
                    var fade2Shifted = sngKaihiTime >> 5;
                    if (fade2Shifted < sptr.Tmp)
                    {
                        sptr.Tmpd -= fade2Shifted;
                    }
                }
            }

            var tmpd = sptr.Tmpd;
            if (tmpd >= 256)
            {
                sptr.Tmpd = tmpd & 0xff;
                --sptr.Ngc;

                if (sptr.Ngc != 0)
                {
                    Keych();
                }
                else if (TxRead() != 0)
                {
                    Keyoff();
                    return 1;
                }

                TempoCh();
                BendCh();
                VolCompute();
            }
            else
            {
                NoteControl();
            }

            if (keyFg != 0)
                Keyon();

            return 0;
        }

        int TxRead()
        {
            var loopCount = 0;
            var readFg = 1;

            while (readFg != 0)
            {
                loopCount++;
                if (loopCount == 256)
                {
                    return 1;
                }

                var mptrVal = script.Packets[mptr / 4];
                mdata1 = (int)mptrVal.Command;
                if (mdata1 == 0)
                {
                    return 1;
                }

                mdata2 = mptrVal.Data2;
                mdata3 = mptrVal.Data3;
                mdata4 = mptrVal.Data4;
                mptr += 4;

                if (mdata1 >= 128)
                {
                    cntlTbl[mdata1 - 128]();
                    if (mdata1 is 0xF2 or 0xF3 or 0xFF)
                    {
                        readFg = 0;
                    }

                    if (mdata1 == 0xFF)
                    {
                        return 1;
                    }
                }
                else
                {
                    if (sptr.Ngg < 0x64 && mdata4 != 0)
                    {
                        keyFg = 1;
                    }

                    readFg = 0;
                    sptr.RestFg = 0;
                    NoteSet();
                }
            }

            return 0;
        }

        void NoteSet()
        {
            sptr.Ngs = mdata2;
            sptr.Ngg = mdata3;
            sptr.Vol = mdata4 & 0x7F;
            NoteCompute();
            sptr.Ngc = sptr.Ngs;
            var x = sptr.Ngg * sptr.Ngc / 100;

            if (x == 0)
            {
                x = 1;
            }

            sptr.Ngo = x;
        }

        void AdsrReset()
        {
            spuTrWk[mtrack].Rr = sptr.Rrd;
            spuTrWk[mtrack].Env3Fg = 1;
        }

        void NoteCompute()
        {
            int x;

            if (mdata1 >= 0x48)
            {
                DrumSet(mdata1);
                x = 0x24;
            }
            else
            {
                x = mdata1;
            }

            x += sptr.Ptps;
            x = (x << 8) + sptr.Tund;
            x = x + sptr.Lp1Freq + sptr.Lp2Freq;

            while (x >= 0x6000)
            {
                x -= 0x6000;
            }

            var swpEx = sptr.Swpd;

            var pSound = sptr;
            pSound.Vibcc = 0;
            pSound.Vibhc = 0;
            pSound.Swpd = x;

            sptr.VibTmpCnt = 0;
            sptr.VibTblCnt = 0;

            pSound = sptr;
            pSound.Trehc = 0;
            pSound.Trec = 0;
            pSound.Vibd = 0;

            spuTrWk[mtrack].Rr = sptr.Rrd;
            spuTrWk[mtrack].Env3Fg = 1;

            sptr.Swpc = sptr.Swsc;

            if (sptr.Swpc != 0)
            {
                sptr.Swphc = sptr.Swshc;

                if (sptr.Swsk == 0)
                {
                    x = sptr.Swpd;

                    if (sptr.Swss >= 0x7F01)
                    {
                        sptr.Swpd += 0x10000 - (sptr.Swss & 0xFFFF);
                    }
                    else
                    {
                        sptr.Swpd -= sptr.Swss;
                    }

                    Swpadset(x);
                }
                else
                {
                    sptr.Swpm = sptr.Swpd;
                    sptr.Swpd = swpEx;
                }
            }

            FreqSet(sptr.Swpd);
        }

        void Swpadset(int xfreq)
        {
            if (sptr.Swpc == 0)
                return;

            var flameDat = sptr.Swpc << 8;
            flameDat /= sptr.Tmp;

            xfreq = xfreq switch
            {
                < 0 => 0,
                >= 0x6000 => 0x5FFF,
                _ => xfreq
            };

            sptr.Swpm = xfreq;

            xfreq -= sptr.Swpd;

            if (xfreq < 0)
            {
                xfreq = -xfreq / flameDat;
                sptr.Swpad = -xfreq;
            }
            else
            {
                sptr.Swpad = xfreq / flameDat;
            }
        }

        void VolCompute()
        {
            if (sptr.Pvoc != 0)
            {
                if (--sptr.Pvoc == 0)
                {
                    sptr.Pvod = sptr.Pvom << 8;
                }
                else
                {
                    sptr.Pvod += sptr.Pvoad;
                }
            }

            if (sptr.Vol != 0)
            {
                int depth;

                if (sptr.Tred == 0)
                {
                    depth = 0;
                }
                else
                {
                    if (sptr.Trehs == sptr.Trehc)
                    {
                        sptr.Trec += sptr.Trecad;
                        var mult = sptr.Trec;

                        depth = mult switch
                        {
                            < 0 => sptr.Tred * -mult,
                            0 => 1,
                            _ => sptr.Tred * mult
                        };
                    }
                    else
                    {
                        sptr.Trehc++;
                        depth = 0;
                    }
                }

                VolXSet(depth >> 8);
            }

            PanGenerate();
        }

        void PanGenerate()
        {
            if (sptr.Panc == 0)
                return;

            if (--sptr.Panc == 0)
                sptr.Pand = sptr.Panm;
            else
                sptr.Pand += sptr.Panad;

            sptr.Panf = sptr.Pand >> 8;
        }

        void KeyCutOff()
        {
            if (sptr.Rrd <= 7)
                return;

            spuTrWk[mtrack].Rr = 7;
            spuTrWk[mtrack].Env3Fg = 1;
        }

        void Keych()
        {
            if (sptr.Ngg < 0x64u && sptr.Ngc == 1 && (short)sptr.Rrd >= 8u)
            {
                spuTrWk[mtrack].Rr = 7;
                spuTrWk[mtrack].Env3Fg = 1;
            }

            if (sptr.Ngo != 0)
            {
                sptr.Ngo--;

                if (sptr.Ngo == 0)
                    Keyoff();
            }

            var setFg = 0;

            var swpc = sptr.Swpc;
            if (swpc != 0)
            {
                var swphc = sptr.Swphc;
                if (swphc != 0)
                {
                    sptr.Swphc--;
                }
                else
                {
                    if (sptr.Swsk == 0)
                    {
                        sptr.Swpc = swpc - 1;

                        if (((swpc - 1) & 0xFF) == 0)
                            sptr.Swpd = sptr.Swpm;
                        else
                            sptr.Swpd += sptr.Swpad;
                    }
                    else
                    {
                        PorCompute();
                    }

                    setFg = 1;
                }
            }

            var vibdm = sptr.Vibdm;
            var vibData = 0;

            if (vibdm != 0)
            {
                var vibhc = sptr.Vibhc;
                if (vibhc != sptr.Vibhs)
                {
                    sptr.Vibhc = vibhc + 1;
                }
                else
                {
                    if (sptr.Vibcc == sptr.Vibcs)
                    {
                        sptr.Vibd = vibdm;
                    }
                    else
                    {
                        if (sptr.Vibcc != 0)
                        {
                            sptr.Vibd += sptr.Vibad;
                        }
                        else
                        {
                            sptr.Vibd = sptr.Vibad;
                        }

                        ++sptr.Vibcc;
                    }

                    sptr.VibTmpCnt += sptr.Vibcad;
                    if (sptr.VibTmpCnt >= 256)
                    {
                        sptr.VibTmpCnt &= 0xFF;
                        vibData = VibCompute();
                        setFg = 1;
                    }
                }
            }

            var rdmData = Random();
            if (rdmData != 0)
            {
                vibData += rdmData;
                setFg = 1;
            }

            if (setFg != 0)
                FreqSet(sptr.Swpd + vibData);
        }

        void PorCompute()
        {
            int pFreqH;
            int pFreqL;

            var porFreq = sptr.Swpm - sptr.Swpd;

            switch (porFreq)
            {
                case < 0:
                {
                    porFreq = -porFreq;
                    pFreqL = porFreq & 0xFF;
                    pFreqH = porFreq >> 8;
                    pFreqL = (pFreqL * sptr.Swsc) >> 8;
                    pFreqH *= sptr.Swsc;
                    porFreq = pFreqH + pFreqL;

                    if (porFreq == 0)
                        porFreq = 1;

                    porFreq = -porFreq;
                    break;
                }
                case 0:
                    sptr.Swpc = 0;
                    break;
                default:
                {
                    pFreqL = porFreq & 0xFF;
                    pFreqH = porFreq >> 8;
                    pFreqL = (pFreqL * sptr.Swsc) >> 8;
                    pFreqH *= sptr.Swsc;
                    porFreq = pFreqH + pFreqL;

                    if (porFreq == 0)
                        porFreq = 1;

                    break;
                }
            }

            sptr.Swpd += porFreq;
        }

        int VibCompute()
        {
            int vibData;

            sptr.VibTblCnt += sptr.VibTcOfst;
            sptr.VibTblCnt &= 0x3F;
            var tblData = vibxTbl[sptr.VibTblCnt & 0x1F];

            var tmp = sptr.Vibd;
            if (0x7FFF >= tmp)
            {
                vibData = (tmp >> 7) & 0xFE;
                vibData = (vibData * tblData) >> 8;
            }
            else
            {
                vibData = ((tmp >> 8) & 0x7F) + 2;
                vibData = (vibData * tblData) >> 1;
            }

            if (sptr.VibTblCnt >= 32u)
            {
                vibData = -vibData;
            }

            return vibData;
        }

        int VibGenerate(int cnt)
        {
            int vibChar;
            int vibData;

            if (cnt << 24 < 0)
            {
                vibChar = -cnt * 2;
                if (-cnt << 25 < 0)
                {
                    vibChar = -vibChar;
                }

                vibData = ((sptr.Vibd >> 8) & 0xff) * (vibChar / 4);
                vibData = -vibData;
            }
            else
            {
                vibChar = cnt * 2;
                if (cnt << 25 < 0)
                {
                    vibChar = -vibChar;
                }

                vibData = ((sptr.Vibd >> 8) & 0xff) * (vibChar / 4);
            }

            if (sptr.Vibdm < 0x8000)
            {
                vibData >>= 2;
            }

            return vibData;
        }

        void BendCh()
        {
            if (sptr.Swpc == 0)
                return;

            var mptrVal = script.Packets[mptr / 4];
            mdata1 = (int)mptrVal.Command;

            if (mdata1 != 0xe4)
                return;

            sptr.Swphc = mptrVal.Data2;
            sptr.Swpc = mptrVal.Data3;
            int bendFrq = mptrVal.Data4;
            mptr += 4;

            bendFrq = (bendFrq + sptr.Ptps) << 8;
            bendFrq += sptr.Tund;

            Swpadset(bendFrq);
        }

        void NoteControl()
        {
            if (sptr.Vol != 0 && sptr.Tred != 0 &&
                sptr.Trehs == sptr.Trehc)
            {
                sptr.Trec += (sptr.Trecad * sptr.Tmpd) >> 8;

                var depth = sptr.Trec switch
                {
                    < 0 => sptr.Tred * -sptr.Trec,
                    0 => 1,
                    _ => sptr.Tred * sptr.Trec
                };

                VolXSet(depth >> 8);
            }

            var fSetFg = 0;
            var frqData = sptr.Swpd;

            if (sptr.Swpc != 0 && sptr.Swphc == 0)
            {
                fSetFg = 1;

                if (sptr.Swsk == 0)
                    sptr.Swpd += sptr.Swpad;
                else
                    PorCompute();

                frqData = sptr.Swpd;
            }

            if (sptr.Vibd != 0 && sptr.Vibhs == sptr.Vibhc)
            {
                sptr.VibTmpCnt += sptr.Vibcad;

                if (sptr.VibTmpCnt >= 256)
                {
                    sptr.VibTmpCnt &= 0xFF;
                    frqData += VibCompute();
                    fSetFg = 1;
                }
            }

            var rdmData = Random();

            if (rdmData != 0)
            {
                fSetFg = 1;
                frqData += rdmData;
            }

            if (fSetFg != 0)
            {
                FreqSet(frqData);
            }
        }

        int Random()
        {
            var frqDt = 0;

            if (sptr.Rdms == 0)
                return frqDt;

            sptr.Rdmc += sptr.Rdms;

            if (sptr.Rdmc <= 256)
                return frqDt;

            sptr.Rdmc &= 255;
            sptr.Rdmo++;
            sptr.Rdmo &= 0x7F;

            var temp2 = rdmTbl[sptr.Rdmo];
            frqDt = rdmTbl[sptr.Rdmo + 1] << 8;
            frqDt += temp2;
            frqDt &= sptr.Rdmd;

            return frqDt;
        }

        void TempoCh()
        {
            if (sptr.Tmpc == 0)
                return;

            if (--sptr.Tmpc == 0)
                sptr.Tmpw = sptr.Tmpm << 8;
            else
                sptr.Tmpw += sptr.Tmpad;

            sptr.Tmp = sptr.Tmpw >> 8;
        }

        void VolXSet(int depth)
        {
            var volData = sptr.Vol;
            volData -= depth;
            volData += sptr.Lp1Vol;
            volData += sptr.Lp2Vol;
            volData = volData switch
            {
                < 0 => 0,
                >= 128 => 127,
                _ => volData
            };

            var pvodW = (sptr.Pvod >> 8) & 0xFF;
            VolSet(((pvodW * volData) >> 8) & 0xFF);
        }

        void RestSet()
        {
            sptr.RestFg = 1;
            Keyoff();
            sptr.Ngs = mdata2;
            sptr.Ngg = 0;
            sptr.Vol = 0;
            sptr.Ngc = sptr.Ngs;
            sptr.Ngo = 0;
        }

        void TieSet()
        {
            sptr.RestFg = 1;
            sptr.Ngs = mdata2;
            sptr.Ngg = mdata3;
            sptr.Ngc = sptr.Ngs;
            var temp1 = sptr.Ngg * sptr.Ngc / 100;
            if (temp1 == 0)
            {
                temp1 = 1;
            }

            sptr.Ngo = temp1;
        }

        void SnoSet()
        {
            sptr.Snos = mdata2;
            Keyoff();
            ToneSet(mdata2);
        }

        void SvlSet()
        {
            sptr.Snos = mdata2;
            Keyoff();
            ToneSet(mdata2);
        }

        void SvpSet()
        {
            sptr.Snos = mdata2;
            Keyoff();
            ToneSet(mdata2);
        }

        void UseSet()
        {
            /* do nothing */
        }

        void PanSet()
        {
            sptr.Panmod = mdata2;
            sptr.Panf = mdata3 + 20;
            sptr.Pand = sptr.Panf << 8;
            sptr.Panc = 0;
        }

        void PanMove()
        {
            sptr.Panc = mdata2;
            var panDShift = mdata3 + 0x14;
            sptr.Panm = panDShift << 8;
            var panData = panDShift - sptr.Panf;

            if (panData < 0)
            {
                sptr.Panad = -((-panData << 8) / mdata2);
                if (sptr.Panad < -2032)
                {
                    sptr.Panad = -2032;
                }
            }
            else
            {
                sptr.Panad = (panData << 8) / mdata2;
                if (sptr.Panad > 2032)
                {
                    sptr.Panad = 2032;
                }
            }
        }

        void VibSet()
        {
            sptr.Vibhs = mdata2;
            sptr.Vibcad = mdata3;

            switch (sptr.Vibcad)
            {
                case < 64 and < 32:
                    sptr.VibTcOfst = 1;
                    sptr.Vibcad <<= 3;
                    break;
                case < 64:
                    sptr.VibTcOfst = 2;
                    sptr.Vibcad <<= 2;
                    break;
                case < 128:
                    sptr.VibTcOfst = 4;
                    sptr.Vibcad <<= 1;
                    break;
                default:
                    sptr.VibTcOfst = sptr.Vibcad != 255 ? 8 : 16;
                    break;
            }

            sptr.Vibd = mdata4 << 8;
            sptr.Vibdm = mdata4 << 8;
        }

        void VibChange()
        {
            sptr.Vibcs = mdata2;
            sptr.Vibad = sptr.Vibdm / mdata2;
        }

        void RdmSet()
        {
            sptr.Rdms = mdata2;
            sptr.Rdmd = (mdata3 << 8) + mdata4;
            sptr.Rdmc = 0;
            sptr.Rdmo = 0;
        }

        void Lp1Start()
        {
            sptr.Lp1Addr = mptr;
            sptr.Lp1Cnt = 0;
            sptr.Lp1Freq = 0;
            sptr.Lp1Vol = 0;
        }

        void Lp1End()
        {
            if (stopJouchuuSe != 0 && mdata2 == 0)
            {
                sptr.Lp1Vol = 0;
                sptr.Lp1Freq = 0;
                ++stopJouchuuSe;
            }
            else
            {
                var v1 = sptr.Lp1Cnt + 1;
                sptr.Lp1Cnt = v1;

                if (v1 != mdata2 || v1 == 0)
                {
                    sptr.Lp1Vol += mdata3;
                    sptr.Lp1Freq += mdata4 * 8;
                    mptr = sptr.Lp1Addr;
                }
                else
                {
                    sptr.Lp1Vol = 0;
                    sptr.Lp1Freq = 0;
                }
            }
        }

        void Lp2Start()
        {
            sptr.Lp2Addr = mptr;
            sptr.Lp2Cnt = 0;
            sptr.Lp2Freq = 0;
            sptr.Lp2Vol = 0;
        }

        void Lp2End()
        {
            var cnt = sptr.Lp2Cnt + 1;
            sptr.Lp2Cnt = cnt;

            if (cnt == mdata2 && cnt != 0)
                return;

            sptr.Lp2Vol += mdata3;
            sptr.Lp2Freq += 8 * mdata4;
            mptr = sptr.Lp2Addr;
        }

        void L3SSet()
        {
            sptr.Lp3Addr = mptr;
        }

        void L3ESet()
        {
            if (sptr.Lp3Addr != 0)
                mptr = sptr.Lp3Addr;
            else
                BlockEnd();
        }

        void TempoSet()
        {
            sptr.Tmp = mdata2;
        }

        void TempoMove()
        {
            sptr.Tmpc = mdata2;
            sptr.Tmpm = mdata3;
            sptr.Tmpw = sptr.Tmp << 8;

            var tmpData = sptr.Tmpm - sptr.Tmp;
            if (tmpData < 0)
            {
                if (tmpData < -127)
                    tmpData = -127;

                sptr.Tmpad = -((-tmpData << 8) / sptr.Tmpc);

                if (sptr.Tmpad < -2032)
                    sptr.Tmpad = -2032;
            }
            else
            {
                if (tmpData > 127)
                    tmpData = 127;

                sptr.Tmpad = (tmpData << 8) / sptr.Tmpc;

                if (sptr.Tmpad > 0x7F0)
                    sptr.Tmpad = 0x7F0;
            }
        }

        void TransSet()
        {
            sptr.Ptps = mdata2;
        }

        void TreSet()
        {
            sptr.Trehs = mdata2;
            sptr.Trecad = mdata3;
            sptr.Tred = mdata4;
        }

        void VolChg()
        {
            sptr.Pvod = mdata2 << 8;
            sptr.Pvoc = 0;
        }

        void VolMove()
        {
            sptr.Pvoc = mdata2;
            sptr.Pvom = mdata3;
            var volData = mdata3 << 8;
            volData -= sptr.Pvod;
            if (volData < 0)
            {
                sptr.Pvoad = -(-volData / sptr.Pvoc);
                if (sptr.Pvoad < -2032)
                    sptr.Pvoad = -2032;
            }
            else
            {
                sptr.Pvoad = volData / sptr.Pvoc;
                if (sptr.Pvoad > 0x7F0)
                    sptr.Pvoad = 0x7F0;
            }
        }

        void PorSet()
        {
            sptr.Swshc = 0;
            sptr.Swsc = mdata2;
            sptr.Swsk = mdata2 == 0 ? 0 : 1;
        }

        void SwsSet()
        {
            sptr.Swsk = 0;
            sptr.Swshc = mdata2;
            sptr.Swsc = mdata3;
            sptr.Swss = mdata4 << 8;
        }

        void DetuneSet()
        {
            sptr.Tund = mdata2 << 2;
        }

        void SwpSet()
        {
            /* do nothing */
        }

        void EchoSet1()
        {
            /* do nothing */
        }

        void EchoSet2()
        {
            /* do nothing */
        }

        void EonSet()
        {
            if (mtrack - 13 < 8 && sePlaying[mtrack - 13].Kind == 0)
                eons |= spuChTbl[mtrack + 1];
        }

        void EofSet()
        {
            if (mtrack - 13 < 8 && sePlaying[mtrack - 13].Kind == 0)
                eoffs |= spuChTbl[mtrack + 1];
        }

        void KakkoStart()
        {
            sptr.Kak1Ptr = mptr;
            sptr.Kakfg = 0;
        }

        void KakkoEnd()
        {
            switch (sptr.Kakfg)
            {
                case 0:
                    sptr.Kakfg++;
                    break;

                case 1:
                    sptr.Kakfg++;
                    sptr.Kak2Ptr = mptr;
                    mptr = sptr.Kak1Ptr;
                    break;

                case 2:
                    sptr.Kakfg--;
                    mptr = sptr.Kak2Ptr;
                    break;
            }
        }

        void EnvSet()
        {
            /* do nothing */
        }

        void AdsSet()
        {
            spuTrWk[mtrack].AMode = 1;
            spuTrWk[mtrack].Ar = ~mdata2 & 0x7F;
            spuTrWk[mtrack].Dr = ~mdata3 & 0xF;
            spuTrWk[mtrack].Sl = mdata4 & 0xF;
            spuTrWk[mtrack].Env1Fg = 1;
        }

        void SrsSet()
        {
            spuTrWk[mtrack].SMode = 3;
            spuTrWk[mtrack].Sr = ~mdata2 & 0x7F;
            spuTrWk[mtrack].Env2Fg = 1;
        }

        void RrsSet()
        {
            var flags = ~mdata2 & 0x1F;
            spuTrWk[mtrack].RMode = 3;
            spuTrWk[mtrack].Rr = flags;
            sptr.Rrd = flags;
            spuTrWk[mtrack].Env3Fg = 1;
        }

        void PmSet()
        {
            /* do nothing */
        }

        void JumpSet()
        {
            /* do nothing */
        }

        void BlockEnd()
        {
            keyoffs |= keyd;
        }

        void NoCmd()
        {
            /* do nothing */
        }
    }
}