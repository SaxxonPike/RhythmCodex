using RhythmCodex.Infrastructure;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Xact.Model;

[Model]
public struct XwbMiniWaveFormat : IWaveFormat
{
    public int Value { get; set; }

    public int BitsPerSample
    {
        get
        {
            return wFormatTag switch
            {
                XwbConstants.WavebankminiformatTagXma or XwbConstants.WavebankminiformatTagWma => 2 * 8,
                XwbConstants.WavebankminiformatTagAdpcm => 4,
                _ => wBitsPerSample == XwbConstants.WavebankminiformatBitdepth16 ? 16 : 8
            };
        }
    }

    public int BlockAlign
    {
        get
        {
            switch (wFormatTag)
            {
                case XwbConstants.WavebankminiformatTagPcm:
                    return wBlockAlign;
                case XwbConstants.WavebankminiformatTagXma:
                    return nChannels * 8 * 2 / 8;
                case XwbConstants.WavebankminiformatTagAdpcm:
                    return (wBlockAlign + XwbConstants.AdpcmMiniwaveformatBlockalignConversionOffset) * nChannels;
                case XwbConstants.WavebankminiformatTagWma:
                    var dwBlockAlignIndex = wBlockAlign & 0x1F;
                    if (dwBlockAlignIndex < XwbConstants.MaxWmaBlockAlignEntries)
                        return XwbConstants.WmaBlockAlign[dwBlockAlignIndex];
                    break;
            }

            return 0;
        }
    }

    public int ByteRate
    {
        get
        {
            switch (wFormatTag)
            {
                case XwbConstants.WavebankminiformatTagPcm:
                case XwbConstants.WavebankminiformatTagXma:
                    return nSamplesPerSec * wBlockAlign;
                case XwbConstants.WavebankminiformatTagAdpcm:
                    return BlockAlign * nSamplesPerSec / AdpcmSamplesPerBlock;
                case XwbConstants.WavebankminiformatTagWma:
                    var dwBytesPerSecIndex = wBlockAlign >> 5;
                    if (dwBytesPerSecIndex < XwbConstants.MaxWmaAvgBytesPerSecEntries)
                        return XwbConstants.WmaAvgBytesPerSec[dwBytesPerSecIndex];
                    break;
            }

            return 0;
        }
    }

    public int AdpcmSamplesPerBlock
    {
        get
        {
            var nBlockAlign = (wBlockAlign + XwbConstants.AdpcmMiniwaveformatBlockalignConversionOffset) * nChannels;
            return nBlockAlign * 2 / nChannels - 12;
        }
    }

    public int SampleRate => nSamplesPerSec;

    public int Channels => nChannels;

    public int FormatTag => wFormatTag;

    private int wFormatTag
    {
        get => (Value >> 0) & 0x3;
        set
        {
            Value &= ~(0x3 << 0);
            Value |= (value & 0x3) << 0;
        }
    }

    private int nChannels
    {
        get => (Value >> 2) & 0x7;
        set
        {
            Value &= ~(0x7 << 2);
            Value |= (value & 0x7) << 2;
        }
    }

    private int nSamplesPerSec
    {
        get => (Value >> 5) & 0x3FFFF;
        set
        {
            Value &= ~(0x3FFFF << 5);
            Value |= (value & 0x3FFFF) << 5;
        }
    }

    private int wBlockAlign
    {
        get => (Value >> 23) & 0xFF;
        set
        {
            Value &= ~(0xFF << 23);
            Value |= (value & 0xFF) << 23;
        }
    }

    private int wBitsPerSample
    {
        get => (Value >> 31) & 0x1;
        set
        {
            Value &= ~(0x1 << 31);
            Value |= (value & 0x1) << 31;
        }
    }
}