using System.IO;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Xact.Model
{
    public struct WaveBankMiniWaveFormat : IWaveFormat
    {
        public int Value;

        public int BitsPerSample
        {
                
            get
            {
                switch (wFormatTag)
                {
                    case XactConstants.WavebankminiformatTagXma:
                    case XactConstants.WavebankminiformatTagWma:
                        return 2 * 8;
                    case XactConstants.WavebankminiformatTagAdpcm:
                        return 4;
                    default:
                        return wBitsPerSample == XactConstants.WavebankminiformatBitdepth16 ? 16 : 8;
                }
            }
        }

        public int BlockAlign
        {
            get
            {
                switch (wFormatTag)
                {
                    case XactConstants.WavebankminiformatTagPcm:
                        return wBlockAlign;
                    case XactConstants.WavebankminiformatTagXma:
                        return (nChannels * (8 * 2) / 8);
                    case XactConstants.WavebankminiformatTagAdpcm:
                        return (wBlockAlign + XactConstants.AdpcmMiniwaveformatBlockalignConversionOffset) * nChannels;
                    case XactConstants.WavebankminiformatTagWma:
                        var dwBlockAlignIndex = wBlockAlign & 0x1F;
                        if (dwBlockAlignIndex < XactConstants.MaxWmaBlockAlignEntries)
                            return XactConstants.WmaBlockAlign[dwBlockAlignIndex];
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
                    case XactConstants.WavebankminiformatTagPcm:
                    case XactConstants.WavebankminiformatTagXma:
                        return (nSamplesPerSec * wBlockAlign);
                    case XactConstants.WavebankminiformatTagAdpcm:
                        return (BlockAlign * nSamplesPerSec / AdpcmSamplesPerBlock);
                    case XactConstants.WavebankminiformatTagWma:
                        var dwBytesPerSecIndex = wBlockAlign >> 5;
                        if (dwBytesPerSecIndex < XactConstants.MaxWmaAvgBytesPerSecEntries)
                            return XactConstants.WmaAvgBytesPerSec[dwBytesPerSecIndex];
                        break;
                }
                return 0;
            }
        }

        public int AdpcmSamplesPerBlock
        {
            get
            {
                var nBlockAlign = (wBlockAlign + XactConstants.AdpcmMiniwaveformatBlockalignConversionOffset) * nChannels;
                return nBlockAlign * 2 / nChannels - 12;
            }
        }

        public int SampleRate => nSamplesPerSec;

        public int Channels => nChannels;

        public int FormatTag => wFormatTag;

        private int wFormatTag
        {
            get => (Value >> 0) & 0x3;
            set { Value &= ~(0x3 << 0); Value |= (value & 0x3) << 0; }
        }

        private int nChannels
        {
            get => (Value >> 2) & 0x7;
            set { Value &= ~(0x7 << 2); Value |= (value & 0x7) << 2; }
        }

        private int nSamplesPerSec
        {
            get => (Value >> 5) & 0x3FFFF;
            set { Value &= ~(0x3FFFF << 5); Value |= (value & 0x3FFFF) << 5; }
        }

        private int wBlockAlign
        {
            get => (Value >> 23) & 0xFF;
            set { Value &= ~(0xFF << 23); Value |= (value & 0xFF) << 23; }
        }

        private int wBitsPerSample
        {
            get => (Value >> 31) & 0x1;
            set { Value &= ~(0x1 << 31); Value |= (value & 0x1) << 31; }
        }

        public static WaveBankMiniWaveFormat Read(Stream source)
        {
            var reader = new BinaryReader(source);
            var result = new WaveBankMiniWaveFormat {Value = reader.ReadInt32()};
            return result;
        }
    }
}