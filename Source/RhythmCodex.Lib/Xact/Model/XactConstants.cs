namespace RhythmCodex.Xact.Model
{
    public static class XactConstants
    {
        public const int AdpcmMiniwaveformatBlockalignConversionOffset = 22;
        public const int WavebankHeaderSignature = 0x444E4257;
        public const int WavebankHeaderVersion = 44;
        public const int WavebankBanknameLength = 64;
        public const int WavebankEntrynameLength = 64;
        public const int WavebankMaxDataSegmentSize = 0x7FFFFFFF;
        public const int WavebankMaxCompactDataSegmentSize = 0x1FFFFF;
        public const int WavebankTypeBuffer = 0x00000000;
        public const int WavebankTypeStreaming = 0x00000001;
        public const int WavebankTypeMask = 0x00000001;
        public const int WavebankFlagsEntrynames = 0x00010000;
        public const int WavebankFlagsCompact = 0x00020000;
        public const int WavebankFlagsSyncDisabled = 0x00040000;
        public const int WavebankFlagsSeektables = 0x00080000;
        public const int WavebankFlagsMask = 0x000F0000;
        public const int WavebankEntryFlagsReadahead = 0x00000001;
        public const int WavebankEntryFlagsLoopcache = 0x00000002;
        public const int WavebankEntryFlagsRemovelooptail = 0x00000004;
        public const int WavebankEntryFlagsIgnoreloop = 0x00000008;
        public const int WavebankEntryFlagsMask = 0x0000000F;
        public const int WavebankminiformatTagPcm = 0x0;
        public const int WavebankminiformatTagXma = 0x1;
        public const int WavebankminiformatTagAdpcm = 0x2;
        public const int WavebankminiformatTagWma = 0x3;
        public const int WavebankminiformatBitdepth8 = 0x0;
        public const int WavebankminiformatBitdepth16 = 0x0;
        public const int WavebankentryXmastreamsMax = 3;
        public const int WavebankentryXmachannelsMax = 6;
        public const int WavebankDvdSectorSize = 2048;
        public const int WavebankDvdBlockSize = (WavebankDvdSectorSize * 16);
        public const int WavebankAlignmentMin = 4;
        public const int WavebankAlignmentDvd = WavebankDvdSectorSize;
        public const int MaxWmaAvgBytesPerSecEntries = 7;
        public const int MaxWmaBlockAlignEntries = 17;

        public static readonly int[] WmaAvgBytesPerSec =
        {
            12000,
            24000,
            4000,
            6000,
            8000,
            20000,
            2500
        };

        public static readonly int[] WmaBlockAlign =
        {
            929,
            1487,
            1280,
            2230,
            8917,
            8192,
            4459,
            5945,
            2304,
            1536,
            1485,
            1000,
            2731,
            4096,
            6827,
            5462,
            1280
        };
    }
}