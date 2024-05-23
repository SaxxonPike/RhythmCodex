using JetBrains.Annotations;

namespace RhythmCodex;

[PublicAPI]
public interface IResolver
{
    T Resolve<T>() where T : notnull;
}