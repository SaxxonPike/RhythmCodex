using JetBrains.Annotations;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

[PublicAPI]
public interface IBeatmaniaPs2KeyProvider
{
    string GetKeyFor14thStyle();
    string GetKeyFor15thStyle();
    string GetKeyFor16thStyle();
}