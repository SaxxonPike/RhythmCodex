using System.Windows.Forms;
using RhythmCodex.IoC;

namespace RhythmCodex.Gui.Forms
{
    [Service]
    public class FileDialog : IFileDialog
    {
        public string OpenFile(string fileName, string filter, bool multi)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = filter ?? "All files (*.*)|*.*",
                Multiselect = multi
            };

            if (ofd.FileName != null)
                ofd.FileName = fileName;

            return ofd.ShowDialog() == DialogResult.OK 
                ? (multi ? string.Join('|', ofd.FileNames) : ofd.FileName) 
                : null;
        }

        public string SaveFile(string fileName, string filter)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = filter ?? "All files (*.*)|*.*",
                OverwritePrompt = true
            };

            if (sfd.FileName != null)
                sfd.FileName = fileName;

            return sfd.ShowDialog() == DialogResult.OK ? sfd.FileName : null;
        }

        public string OpenFolder(string folder)
        {
            using var fbd = new FolderBrowserDialog
            {
            };

            if (folder != null)
                fbd.SelectedPath = folder;

            return fbd.ShowDialog() == DialogResult.OK ? fbd.SelectedPath : null;
        }
    }
}