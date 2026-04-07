using System;
using System.Runtime.Intrinsics;
using RhythmCodex.Archs.Psx.Models;

namespace RhythmCodex.Archs.Psx.Processors;

public class PsxSpu : IPsxSpu
{
    public Func<int, int>? ReadRequestHandler { get; set; }

    public PsxSpuState State { get; private set; } = new PsxSpuState();

    public short OutL { get; private set; }
    public short OutR { get; private set; }

    //
    // Output levels.
    // 0, 1, 2 are 8 voices each
    // 3[0] is reverb
    // 3[1] is cdda
    // 3[2] is external
    //

    private Vector256<int> _out0L;
    private Vector256<int> _out0R;
    private Vector256<int> _out1L;
    private Vector256<int> _out1R;
    private Vector256<int> _out2L;
    private Vector256<int> _out2R;
    private Vector128<int> _out3L;
    private Vector128<int> _out3R;

    private void MixFinal()
    {
        var left = Vector256.Sum(_out0L) +
                   Vector256.Sum(_out1L) +
                   Vector256.Sum(_out2L) +
                   Vector128.Sum(_out3L);
        var right = Vector256.Sum(_out0R) +
                    Vector256.Sum(_out1R) +
                    Vector256.Sum(_out2R) +
                    Vector128.Sum(_out3R);

        OutL = unchecked((short)Math.Clamp(left, short.MinValue, short.MaxValue));
        OutR = unchecked((short)Math.Clamp(right, short.MinValue, short.MaxValue));
    }

    public void Reset()
    {
        State = new PsxSpuState();
    }

    public void WriteReg(int addr, int value)
    {
        switch (addr)
        {
            case var x when (x & ~0x1F0) == 0x1F801C00 &&
                            (x & 0x1F0) < 0x180 &&
                            (x & 1) == 0:
            {
                var voice = State.Voices[(x & 0x1F0) >> 4];

                switch (x & 0xF)
                {
                    case 0x0:
                    {
                        voice.Gain[0] = unchecked((ushort)value);
                        break;
                    }
                    case 0x2:
                    {
                        voice.Gain[1] = unchecked((ushort)value);
                        break;
                    }
                    case 0x4:
                    {
                        voice.Rate = unchecked((ushort)value);
                        break;
                    }
                    case 0x6:
                    {
                        voice.StartAddress = unchecked((ushort)value);
                        break;
                    }
                    case 0x8:
                    {
                        voice.AdsrMode0 = unchecked((ushort)value);
                        break;
                    }
                    case 0xA:
                    {
                        voice.AdsrMode1 = unchecked((ushort)value);
                        break;
                    }
                    case 0xC:
                    {
                        voice.AdsrLevel = unchecked((ushort)value);
                        break;
                    }
                    case 0xE:
                    {
                        voice.RepeatAddress = unchecked((ushort)value);
                        break;
                    }
                }

                break;
            }
            case 0x1F801D80:
            {
                State.Gain[0] = unchecked((ushort)value);
                break;
            }
            case 0x1F801D82:
            {
                State.Gain[1] = unchecked((ushort)value);
                break;
            }
            case 0x1F801D84:
            {
                State.Reverb.Gain[0] = unchecked((ushort)value);
                break;
            }
            case 0x1F801D86:
            {
                State.Reverb.Gain[1] = unchecked((ushort)value);
                break;
            }
            case 0x1F801D88:
            {
                var temp = value;
                for (var i = 0; i < 16; i++)
                {
                    State.Voices[i].KeyOn = (temp & 1) != 0;
                    temp >>= 1;
                }

                break;
            }
            case 0x1F801D8A:
            {
                var temp = value;
                for (var i = 16; i < 24; i++)
                {
                    State.Voices[i].KeyOn = (temp & 1) != 0;
                    temp >>= 1;
                }

                break;
            }
            case 0x1F801D8C:
            {
                var temp = value;
                for (var i = 0; i < 16; i++)
                {
                    State.Voices[i].KeyOff = (temp & 1) != 0;
                    temp >>= 1;
                }

                break;
            }
            case 0x1F801D8E:
            {
                var temp = value;
                for (var i = 16; i < 24; i++)
                {
                    State.Voices[i].KeyOff = (temp & 1) != 0;
                    temp >>= 1;
                }

                break;
            }
            case 0x1F801D90:
            {
                var temp = value;
                for (var i = 0; i < 16; i++)
                {
                    State.Voices[i].PitchMod = (temp & 1) != 0;
                    temp >>= 1;
                }

                break;
            }
            case 0x1F801D92:
            {
                var temp = value;
                for (var i = 16; i < 24; i++)
                {
                    State.Voices[i].PitchMod = (temp & 1) != 0;
                    temp >>= 1;
                }

                break;
            }
            case 0x1F801D94:
            {
                var temp = value;
                for (var i = 0; i < 16; i++)
                {
                    State.Voices[i].NoiseMode = (temp & 1) != 0;
                    temp >>= 1;
                }

                break;
            }
            case 0x1F801D96:
            {
                var temp = value;
                for (var i = 16; i < 24; i++)
                {
                    State.Voices[i].NoiseMode = (temp & 1) != 0;
                    temp >>= 1;
                }

                break;
            }
            case 0x1F801D98:
            {
                var temp = value;
                for (var i = 0; i < 16; i++)
                {
                    State.Voices[i].ReverbOn = (temp & 1) != 0;
                    temp >>= 1;
                }

                break;
            }
            case 0x1F801D9A:
            {
                var temp = value;
                for (var i = 16; i < 24; i++)
                {
                    State.Voices[i].ReverbOn = (temp & 1) != 0;
                    temp >>= 1;
                }

                break;
            }
            case 0x1F801D9C:
            {
                var temp = value;
                for (var i = 0; i < 16; i++)
                {
                    State.Voices[i].EndX = (temp & 1) != 0;
                    temp >>= 1;
                }

                break;
            }
            case 0x1F801D9E:
            {
                var temp = value;
                for (var i = 16; i < 24; i++)
                {
                    State.Voices[i].EndX = (temp & 1) != 0;
                    temp >>= 1;
                }

                break;
            }
            case 0x1F801DA2:
            {
                State.Reverb.WorkAddress = unchecked((ushort)value);
                break;
            }
            case 0x1F801DA4:
            {
                State.IrqAddress = unchecked((ushort)value);
                break;
            }
            case 0x1F801DA6:
            {
                State.TransferAddress = unchecked((ushort)value);
                break;
            }
            case 0x1F801DA8:
            {
                State.TransferBuffer = unchecked((ushort)value);
                break;
            }
            case 0x1F801DAA:
            {
                State.Control = unchecked((ushort)value);
                break;
            }
            case 0x1F801DAC:
            {
                State.TransferControl = unchecked((ushort)value);
                break;
            }
            case 0x1F801DAE:
            {
                State.Status = unchecked((ushort)value);
                break;
            }
            case 0x1F801DB0:
            {
                State.CddaGain[0] = unchecked((ushort)value);
                break;
            }
            case 0x1F801DB2:
            {
                State.CddaGain[1] = unchecked((ushort)value);
                break;
            }
            case 0x1F801DB4:
            {
                State.ExternalGain[0] = unchecked((ushort)value);
                break;
            }
            case 0x1F801DB6:
            {
                State.ExternalGain[1] = unchecked((ushort)value);
                break;
            }
            case 0x1F801DB8:
            {
                State.CurrentLevel[0] = unchecked((ushort)value);
                break;
            }
            case 0x1F801DBA:
            {
                State.CurrentLevel[1] = unchecked((ushort)value);
                break;
            }
        }
    }
}

public interface IPsxSpu
{
    Func<int, int>? ReadRequestHandler { get; set; }
    void Reset();
    void WriteReg(int addr, int value);
}