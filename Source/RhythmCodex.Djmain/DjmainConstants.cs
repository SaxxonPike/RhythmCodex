namespace RhythmCodex.Djmain
{
    public static class DjmainConstants
    {
        public static readonly int MaxSampleDefinitions = 256;
        public static readonly int DpcmEndMarker = 0x44444444 << 1;
        public static readonly long Pcm8EndMarker = 0x4040404040404040 << 1;
        public static readonly long Pcm16EndMarker = 0x4000400040004000 << 1;

    }
}
