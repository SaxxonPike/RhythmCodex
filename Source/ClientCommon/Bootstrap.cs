using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace ClientCommon
{
    public static class Bootstrap
    {
        public static void StartApp<TApp>(params string[] args) where TApp : class, IApp
        {
            var console = System.Console.Out;

            var container = new ServiceCollection();
            container.AddRhythmCodex(typeof(Bootstrap).Assembly, Assembly.GetCallingAssembly());
            container.AddSingleton<IApp, TApp>();
            container.AddSingleton(System.Console.Out);
            container.AddSingleton<ILogger, TextWriterLogger>();
            container.AddSingleton<ILoggerConfigurationSource, LoggerConfigurationSource>();
            container.AddSingleton<IConsole, Console>();

            var serviceProvider = container.BuildServiceProvider();
            var argsParser = (IArgParser) serviceProvider.GetService(typeof(IArgParser));
            var newArgs = argsParser.Parse(args);

            var app = (IApp) serviceProvider.GetService(typeof(IApp));

            if (newArgs.InputFiles.Any())
            {
                app.Run(console, newArgs);
                SpinWait.SpinUntil(() => ThreadPool.PendingWorkItemCount == 0);
                return;
            }

            console.WriteLine("* usage *");
            console.WriteLine();
            app.Usage(console);
            console.WriteLine();
            console.WriteLine("* global flags *");
            console.WriteLine();
            console.WriteLine("+r          recursive paths");
            console.WriteLine("-o <path>   set output path (such as c:\\output or my_output_folder)");
            console.WriteLine();
        }
    }
}