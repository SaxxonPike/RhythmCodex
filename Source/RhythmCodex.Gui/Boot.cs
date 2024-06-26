using System;
using System.Windows.Forms;
using Autofac;
using ClientCommon;
using RhythmCodex.Cli;
using RhythmCodex.Gui.Forms;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Gui;

internal static class Boot
{
    /// <summary>
    /// Entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        var container = BuildContainer();
        var formFactory = container.Resolve<IFormFactory>();
        var logger = container.Resolve<ILogger>();
            
        logger.Debug("IoC container initialized.");
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        var form = formFactory.CreateMainForm();
        Application.Run(form);
    }

    /// <summary>
    /// Print exception messages including indented inner exception messages.
    /// </summary>
    private static void LogErrors(ILogger logger, Exception exception, int level)
    {
        logger.Error($"{new string(' ', level)}{exception.Message}");
        if (exception.InnerException != null)
            LogErrors(logger, exception.InnerException, level + 2);
    }

    /// <summary>
    /// Build IoC container.
    /// </summary>
    private static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<AppInfrastructureAutofacModule>();
        builder.RegisterModule<AppAutofacModule<App>>();
        builder.RegisterModule<AppAutofacModule<FormFactory>>();
        builder.RegisterModule<AppAutofacModule<ArgParser>>();
        return builder.Build();
    }

}