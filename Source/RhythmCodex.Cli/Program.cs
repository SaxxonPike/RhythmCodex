using RhythmCodex.Cli.Modules;
using RhythmCodex.Ioc;

namespace RhythmCodex.Cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var resolver = new Resolver();
            
            var modules = new ICliModule[]
            {
                resolver.Resolve<SsqCliModule>()
            };
            
            resolver.Resolve<App>().Run(args, modules);
        }
    }
}
