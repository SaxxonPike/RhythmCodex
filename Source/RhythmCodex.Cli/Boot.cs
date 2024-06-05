using System;
using System.Diagnostics;
using Autofac;
using ClientCommon;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli;

internal static class Boot
{
    /// <summary>
    /// Entry point for the application.
    /// </summary>
    private static void Main(string[] args)
    {
        var container = BuildContainer();
        var app = container.Resolve<IApp>();
        var logger = container.Resolve<ILogger>();

        logger.Debug("IoC container initialized.");

        if (Debugger.IsAttached)
        {
            // The outer exception handler is not installed if we are debugging.
            // This makes debugging in the IDE easier.
            app.Run(args);
        }
        else
        {
            try
            {
                app.Run(args);
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                LogErrors(logger, e, 0);
                logger.Warning("An error occurred and the application cannot continue.");
            }
        }
    }

    /// <summary>
    /// Print exception messages including indented inner exception messages.
    /// </summary>
    private static void LogErrors(ILogger logger, Exception exception, int level)
    {
        while (true)
        {
            logger.Error($"{new string(' ', level)}{exception.Message}");
            if (exception.InnerException != null)
            {
                exception = exception.InnerException;
                level += 2;
                continue;
            }

            break;
        }
    }

    /// <summary>
    /// Build IoC container.
    /// </summary>
    private static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<AppInfrastructureAutofacModule>();
        builder.RegisterModule<AppAutofacModule<App>>();
        builder.RegisterModule<AppAutofacModule<ArgParser>>();
        return builder.Build();
    }
}