using System.Linq;
using RhythmCodex.IoC;

namespace ClientCommon;

[Service]

public class ArgResolver(IFileSystem fileSystem) 
    : IArgResolver
{
    /// <summary>
    /// Get all input files from command line args.
    /// </summary>
    public string[] GetInputFiles(Args args)
    {
        return args.InputFiles
            .SelectMany(a =>
                fileSystem.GetFileNames(a, args.RecursiveInputFiles))
            .ToArray();
    }

    /// <summary>
    /// Get output directory from command line args.
    /// </summary>
    public string GetOutputDirectory(Args args)
    {
        return !string.IsNullOrEmpty(args.OutputPath)
            ? args.OutputPath
            : fileSystem.CurrentPath;
    }
}