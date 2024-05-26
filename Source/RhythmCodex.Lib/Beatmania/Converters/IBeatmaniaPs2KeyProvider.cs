using JetBrains.Annotations;

namespace RhythmCodex.Beatmania.Converters;

[PublicAPI]
public interface IBeatmaniaPs2KeyProvider
{
    string GetKeyFor14thStyle();
    string GetKeyFor15thStyle();
    string GetKeyFor16thStyle();
}