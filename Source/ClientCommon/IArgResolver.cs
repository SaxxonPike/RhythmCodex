using JetBrains.Annotations;

namespace ClientCommon;

[PublicAPI]
public interface IArgResolver
{
    string[] GetInputFiles(Args args);
    string GetOutputDirectory(Args args);
}