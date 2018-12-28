namespace RhythmCodex.Cli.Orchestration.Infrastructure
{
    public interface ITaskFactory
    {
        DdrTaskBuilder BuildDdrTask();
    }
}