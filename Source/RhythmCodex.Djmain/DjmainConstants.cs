namespace RhythmCodex.Djmain
{
    public static class DjmainConstants
    {
        public const int DpcmEndMarker = 0x44444444 << 1;
        public const long Pcm8EndMarker = 0x4040404040404040 << 1;
        public const long Pcm16EndMarker = 0x4000400040004000 << 1;
    }
}
