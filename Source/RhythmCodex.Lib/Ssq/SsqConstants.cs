namespace RhythmCodex.Ssq;

public static class SsqConstants
{
    public const int DefaultRate = 150;
    public const int MeasureLength = 4096;
    
    public static class Parameter0
    {
        public static short Timings => 0x0001;
        public static short Triggers => 0x0002;
        public static short Steps => 0x0003;
        public static short Meta => 0x0009;
    }
}