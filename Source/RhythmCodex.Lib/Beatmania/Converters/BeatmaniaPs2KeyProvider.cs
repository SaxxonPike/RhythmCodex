using System.Text;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Converters;

[Service]
public class BeatmaniaPs2KeyProvider : IBeatmaniaPs2KeyProvider
{
    // credit: windyfairy
    // reference: https://github.com/windyfairy/iidx-ps2tools/blob/master/plugins

    private static readonly string[] KeyParts14 =
    {
        "FIRE FIRE", // b0 0
        "Blind Justice", // b4 1
        "earth-like planet", // b8 2
        "2hot2eat", // bc 3
        "op.31", // c0 4
        "X-rated", // c4 5
        "Sense 2007", // c8 6
        "Cyber Force", // cc 7
        "ANDROMEDA II", // d0 8
        "heaven above", // d4 9
    };

    private static readonly string[] KeyParts15 =
    {
        "Blue Rain", // 70 0
        "oratio", // 74 1
        "Digitank System", // 78 2
        "four pieces of heaven", // 7c 3
        "2 tribe 4 K", // 80 4
        "end of world", // 84 5
        "Darling my LUV", // 88 6
        "MENDES", // 8c 7
        "TRIP MACHINE PhoeniX", // 90 8
        "NEW GENERATION", // 94 9
    };

    private static readonly string[] KeyParts16 =
    {
        "PERFECT FULL COMBO HARD EASY", // c0 0
        "ASSIST CLEAR PLAY", // c4 1
        "RANDOM MIRROR", // c8 2
        "Auto Scratch 5Keys", // cc 3
        "DOUBLE BATTLE Win Lose", // d0 4
        "Hi-Speed Flip", // d4 5
        "Normal Hyper Another", // d8 6
        "Beginner Tutorial", // dc 7
        "ECHO REVERB EQ ONLY", // e0 8
        "STANDARD EXPERT CLASS", // e4 9
    };

    public string GetKeyFor14thStyle()
    {
        var key = new StringBuilder();
        key.Append(KeyParts14[1][8]);
        key.Append(KeyParts14[0][3]);
        key.Append(KeyParts14[2][8]);
        key.Append(KeyParts14[3][4]);
        key.Append(KeyParts14[0][1]);
        key.Append(KeyParts14[4][4]);
        key.Append(KeyParts14[5][0]);
        key.Append('q');
        key.Append(KeyParts14[6][9]);
        key.Append(KeyParts14[7][2]);
        key.Append('z');
        key.Append(KeyParts14[6][8]);
        key.Append('9');
        key.Append(KeyParts14[8][5]);
        key.Append(KeyParts14[1][9]);
        key.Append(KeyParts14[9][3]);
        return key.ToString();
    }

    public string GetKeyFor15thStyle()
    {
        var key = new StringBuilder();
        key.Append(KeyParts15[0][3]);
        key.Append('q');
        key.Append(KeyParts15[2][7]);
        key.Append(KeyParts15[0][0]);
        key.Append(KeyParts15[0][7]);
        key.Append(KeyParts15[3][5]);
        key.Append('x');
        key.Append(KeyParts15[4][0]);
        key.Append(KeyParts15[5][7]);
        key.Append(KeyParts15[6][6]);
        key.Append(KeyParts15[7][1]);
        key.Append('x');
        key.Append(KeyParts15[6][12]);
        key.Append(KeyParts15[8][6]);
        key.Append(KeyParts15[8][9]);
        key.Append(KeyParts15[9][4]);
        return key.ToString();
    }

    public string GetKeyFor16thStyle()
    {
        var key = new StringBuilder();
        key.Append(KeyParts16[7][10]);
        key.Append(KeyParts16[8][18]);
        key.Append(KeyParts16[5][10]);
        key.Append(KeyParts16[0][3]);
        key.Append(KeyParts16[7][2]);
        key.Append(KeyParts16[8][7]);
        key.Append('w');
        key.Append(KeyParts16[2][4]);
        key.Append(KeyParts16[5][4]);
        key.Append(KeyParts16[3][8]);
        key.Append(KeyParts16[8][13]);
        key.Append(KeyParts16[6][3]);
        key.Append(KeyParts16[9][10]);
        key.Append(KeyParts16[5][7]);
        key.Append(KeyParts16[1][3]);
        key.Append(KeyParts16[1][11]);
        return key.ToString();
    }
}