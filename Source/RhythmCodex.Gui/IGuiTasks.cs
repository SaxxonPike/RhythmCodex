namespace RhythmCodex.Gui;

public interface IGuiTasks
{
    void DdrExtract573Flash(string gamePath, string cardPath, string outPath);
    void DdrDecrypt573Audio(string files, string outPath, bool decodeNames);
    void SsqDecode(string files, string outPath, double offset);
    void BeatmaniaDecodeDjmainHdd(string files, string outPath, bool skipAudio, bool skipCharts, bool rawCharts);
    void BmsRender(string files, string outPath);
    void BeatmaniaRenderDjmainGst(string files, string outPath);
    void ArcExtract(string files, string outPath);
    void HbnExtract(string files, string outPath);
}