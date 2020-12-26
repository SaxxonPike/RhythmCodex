namespace RhythmCodex.Gui
{
    public interface IGuiTasks
    {
        void DdrExtract573Flash(string gamePath, string cardPath, string outPath);
        void DdrDecrypt573Audio(string files, string outPath);
    }
}