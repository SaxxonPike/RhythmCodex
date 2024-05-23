using System.IO;
using JetBrains.Annotations;

namespace ClientCommon;

[PublicAPI]
public interface IApp
{
    void Run(TextWriter log, Args args);
    void Usage(TextWriter log);
}