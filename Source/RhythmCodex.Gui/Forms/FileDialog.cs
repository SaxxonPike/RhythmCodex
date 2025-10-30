using System.Windows.Forms;
using RhythmCodex.IoC;

namespace RhythmCodex.Gui.Forms;

[Service]
public class FileDialog : IFileDialog
{
    public string? OpenFile(string fileName, string? filter, bool multi)
    {
        using var ofd = new OpenFileDialog();
        ofd.Filter = filter ?? "All files (*.*)|*.*";
        ofd.Multiselect = multi;
        ofd.FileName = fileName;

        return ofd.ShowDialog() == DialogResult.OK
            ? multi 
                ? string.Join('|', ofd.FileNames) 
                : ofd.FileName
            : null;
    }

    public string? SaveFile(string fileName, string? filter)
    {
        using var sfd = new SaveFileDialog();
        sfd.Filter = filter ?? "All files (*.*)|*.*";
        sfd.OverwritePrompt = true;
        sfd.FileName = fileName;

        return sfd.ShowDialog() == DialogResult.OK 
            ? sfd.FileName 
            : null;
    }

    public string? OpenFolder(string? folder)
    {
        using var fbd = new FolderBrowserDialog();
        fbd.AutoUpgradeEnabled = true;
        fbd.ShowNewFolderButton = true;

        if (folder != null)
            fbd.SelectedPath = folder;

        return fbd.ShowDialog() == DialogResult.OK 
            ? fbd.SelectedPath 
            : null;
    }
}