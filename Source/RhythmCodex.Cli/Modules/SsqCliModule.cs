using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Cli.Modules
{
    public class SsqCliModule : ICliModule
    {
        private readonly TextWriter _logger;

        public SsqCliModule(TextWriter logger)
        {
            _logger = logger;
        }

        public string Name => "SSQ";
        
        public string Description => "Encodes and decodes the SSQ format.";

        public IEnumerable<ICommand> Commands => new ICommand[]
        {
            new Command
            {
                Name = "encode",
                Description = "Encodes an SSQ file.",
                Execute = Encode
            }
        };

        private void Encode(IDictionary<string, string[]> args)
        {
            _logger.WriteLine("Todo: write encoder.");
        }

        private void Decode(IDictionary<string, string[]> args)
        {
            _logger.WriteLine("Todo: write decoder.");
        }
    }
}
