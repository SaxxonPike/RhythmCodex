namespace RhythmCodex.Wav.Models
{
    public static class MicrosoftAdpcmConstants
    {
        public static readonly int[] AdaptationTable =
        {
            230, 230, 230, 230, 307, 409, 512, 614,
            768, 614, 512, 409, 307, 230, 230, 230
        };

        public static readonly int[] DefaultCoefficients =
        {
            256, 0,
            512, -256,
            0, 0,
            192, 64,
            240, 0,
            460, -208,
            392, -232
        };

        public static readonly int[] IndexTable =
        {
            0, 1, 2, 3, 4, 5, 6, 7, -8, -7, -6, -5, -4, -3, -2, -1
        };

    }
}