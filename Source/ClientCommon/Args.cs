using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace ClientCommon;

public sealed class Args
{
    public Args(IDictionary<string, string[]> options)
    {
        var opts = options.ToDictionary(kv => kv.Key, kv => kv.Value);
        Options = opts;

        OutputPath = opts.TryGetValue("o", out var opt)
            ? opt.Last()
            : null;

        opts.Remove("o");
            
        InputFiles = opts.TryGetValue(string.Empty, out var value) ? value : [];

        opts.Remove(string.Empty);

        RecursiveInputFiles = opts.ContainsKey("+r");
        opts.Remove("+r");

        FilesAreZipArchives = opts.ContainsKey("+zip");
        opts.Remove("+zip");

        if (opts.TryGetValue("log", out opt))
        {
            LogLevel = opt.FirstOrDefault()?.ToLowerInvariant() switch
            {
                "debug" => LoggerVerbosityLevel.Debug,
                "info" => LoggerVerbosityLevel.Info,
                "warning" => LoggerVerbosityLevel.Warning,
                "error" => LoggerVerbosityLevel.Error,
                _ => LogLevel
            };
        }
    }
        
    public IReadOnlyDictionary<string, string[]> Options { get; }
    public string? OutputPath { get; }
    public IReadOnlyList<string> InputFiles { get; }
    public bool RecursiveInputFiles { get; }
    public bool FilesAreZipArchives { get; }
    public LoggerVerbosityLevel LogLevel { get; } = LoggerVerbosityLevel.Info;
}