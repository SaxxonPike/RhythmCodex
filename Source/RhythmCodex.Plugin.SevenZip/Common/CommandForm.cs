namespace RhythmCodex.Plugin.SevenZip.Common;

public class CommandForm
{
    public string IDString = "";
    public bool PostStringMode = false;
    public CommandForm(string idString, bool postStringMode)
    {
        IDString = idString;
        PostStringMode = postStringMode;
    }
}