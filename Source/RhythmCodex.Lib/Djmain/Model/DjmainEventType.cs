namespace RhythmCodex.Djmain.Model
{
    public enum DjmainEventType
    {
        Marker = 0x0,
        SoundSelect = 0x1,
        Bpm = 0x2,
        End = 0x4,
        Bgm = 0x5,
        JudgeTiming = 0x6,
        JudgeSound = 0x7,
        JudgeTrigger = 0x8,
        PhraseSelect = 0x9
    }
}
