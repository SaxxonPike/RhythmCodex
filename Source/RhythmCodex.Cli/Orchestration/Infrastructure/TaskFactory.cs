using RhythmCodex.Cli.Helpers;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Streamers;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Orchestration.Infrastructure
{
    [Service]
    public class TaskFactory : ITaskFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IDdr573StreamReader _ddr573StreamReader;
        private readonly IDdr573Decoder _ddr573Decoder;

        public TaskFactory(
            IFileSystem fileSystem,
            ILogger logger,
            IDdr573StreamReader ddr573StreamReader,
            IDdr573Decoder ddr573Decoder)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _ddr573StreamReader = ddr573StreamReader;
            _ddr573Decoder = ddr573Decoder;
        }
        
        public DdrTaskBuilder BuildDdrTask() => 
            new DdrTaskBuilder(_fileSystem, _logger, _ddr573StreamReader, _ddr573Decoder);
    }
}