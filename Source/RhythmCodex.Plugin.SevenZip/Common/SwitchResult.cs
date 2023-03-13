using System.Collections;

namespace RhythmCodex.Plugin.SevenZip.Common;

public class SwitchResult
{
    public bool ThereIs;
    public bool WithMinus;
    public ArrayList PostStrings = new();
    public int PostCharIndex;
    public SwitchResult()
    {
        ThereIs = false;
    }
}