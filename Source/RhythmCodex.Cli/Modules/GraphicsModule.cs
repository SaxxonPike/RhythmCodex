using System.Collections.Generic;
using ClientCommon;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Cli.Modules;

[Service]
public class GraphicsModule : ICliModule
{
    private readonly ITaskFactory _taskFactory;

    /// <summary>
    /// Create an instance of the graphics module.
    /// </summary>
    public GraphicsModule(
        ITaskFactory taskFactory)
    {
        _taskFactory = taskFactory;
    }

    /// <inheritdoc />
    public string Name => "gfx";

    /// <inheritdoc />
    public string Description => "Handles conversion of standard graphics formats.";

    /// <inheritdoc />
    public IEnumerable<ICommand> Commands => new ICommand[]
    {
        new Command
        {
            Name = "decode-dds",
            Description = "Decodes a DDS image.",
            TaskFactory = DecodeDds
        },
        new Command
        {
            Name = "decode-tga",
            Description = "Decodes a TGA image.",
            TaskFactory = DecodeTga
        },
        new Command
        {
            Name = "decode-tim",
            Description = "Decodes a TIM image.",
            TaskFactory = DecodeTim
        }
    };

    private ITask DecodeTim(Args args)
    {
        return _taskFactory
            .BuildGraphicsTask()
            .WithArgs(args)
            .CreateDecodeTim();
    }

    private ITask DecodeDds(Args args)
    {
        return _taskFactory
            .BuildGraphicsTask()
            .WithArgs(args)
            .CreateDecodeDds();
    }

    private ITask DecodeTga(Args args)
    {
        return _taskFactory
            .BuildGraphicsTask()
            .WithArgs(args)
            .CreateDecodeTga();
    }
}