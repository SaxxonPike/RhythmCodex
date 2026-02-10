using System.Collections.Generic;
using ClientCommon;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Modules;

[Service]
public class XboxModule : ICliModule
{
    private readonly ITaskFactory _taskFactory;

    /// <summary>
    /// Create an instance of the Xbox module.
    /// </summary>
    public XboxModule(
        ITaskFactory taskFactory)
    {
        _taskFactory = taskFactory;
    }

    /// <inheritdoc />
    public string Name => "xbox";

    /// <inheritdoc />
    public string Description => "Handles conversion of Xbox native media.";

    /// <inheritdoc />
    public IEnumerable<ICommand> Commands =>
    [
        new Command
        {
            Name = "decode-xst",
            Description = "Decodes a raw blob of Xbox ADCPM data.",
            TaskFactory = DecodeAdpcm
        },
        new Command
        {
            Name = "extract-xwb",
            Description = "Extracts an XWB sound bank.",
            TaskFactory = ExtractXwb
        },
        new Command
        {
            Name = "extract-iso",
            Description = "Extracts files from an Xbox ISO.",
            TaskFactory = ExtractXiso
        },
        new Command
        {
            Name = "extract-sng",
            Description = "Extracts songs from an SNG file.",
            TaskFactory = ExtractSng
        },
        new Command
        {
            Name = "extract-hbn",
            Description = "Extracts files using an HBN index.",
            TaskFactory = ExtractHbn
        }
    ];

    private ITask DecodeAdpcm(Args args)
    {
        return _taskFactory
            .BuildXboxTask()
            .WithArgs(args)
            .CreateDecodeXst();
    }

    private ITask ExtractXwb(Args args)
    {
        return _taskFactory
            .BuildXboxTask()
            .WithArgs(args)
            .CreateExtractXwb();
    }

    private ITask ExtractXiso(Args args)
    {
        return _taskFactory
            .BuildXboxTask()
            .WithArgs(args)
            .CreateExtractXiso();
    }

    private ITask ExtractSng(Args args)
    {
        return _taskFactory
            .BuildXboxTask()
            .WithArgs(args)
            .CreateExtractSng();
    }

    private ITask ExtractHbn(Args args)
    {
        return _taskFactory
            .BuildXboxTask()
            .WithArgs(args)
            .CreateExtractHbn();
    }
}