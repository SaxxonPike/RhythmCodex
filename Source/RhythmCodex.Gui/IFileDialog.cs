namespace RhythmCodex.Gui
{
    public interface IFileDialog
    {
        string OpenFile(string fileName, string filter, bool multi);
        string SaveFile(string fileName, string filter);
        string OpenFolder(string folder);
    }
}