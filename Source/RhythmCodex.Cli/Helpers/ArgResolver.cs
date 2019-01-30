using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Helpers
{
    [Service]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ArgResolver : IArgResolver
    {
        private readonly IFileSystem _fileSystem;

        public ArgResolver(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        /// <summary>
        /// Get all input files from command line args.
        /// </summary>
        public string[] GetInputFiles(Args args)
        {
            return args.InputFiles.SelectMany(a => _fileSystem.GetFileNames(a, args.RecursiveInputFiles)).ToArray();
        }

        /// <summary>
        /// Get output directory from command line args.
        /// </summary>
        public string GetOutputDirectory(Args args)
        {
            return !string.IsNullOrEmpty(args.OutputPath)
                ? args.OutputPath
                : _fileSystem.CurrentPath;
        }
        
    }
}