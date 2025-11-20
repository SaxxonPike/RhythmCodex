using System.Collections.Generic;

namespace RhythmCodex.Sounds.Wav.Models;

public static class MicrosoftAdpcmConstants
{
    public const int AdaptationTableMinimumSize = 16;

    public static List<int> CreateAdaptationTable() =>
    [
        230, 230, 230, 230, 307, 409, 512, 614,
        768, 614, 512, 409, 307, 230, 230, 230
    ];

    public const int CoefficientTableMinimumSize = 14;

    public static List<int> CreateCoefficientTable() =>
    [
        256, 0,
        512, -256,
        0, 0,
        192, 64,
        240, 0,
        460, -208,
        392, -232
    ];
}