namespace RhythmCodex.Cli.Orchestration.Infrastructure;

public interface ITaskFactory
{
    BeatmaniaTaskBuilder BuildBeatmaniaTask();
    DdrTaskBuilder BuildDdrTask();
    XboxTaskBuilder BuildXboxTask();
    GraphicsTaskBuilder BuildGraphicsTask();
    CompressionTaskBuilder BuildCompressionTask();
    BmsTaskBuilder BuildBmsTask();
    ArcTaskBuilder BuildArcTask();
}