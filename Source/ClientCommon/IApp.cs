using System.IO;

namespace ClientCommon
{
    public interface IApp
    {
        void Run(TextWriter log, Args args);
        void Usage(TextWriter log);
    }
}