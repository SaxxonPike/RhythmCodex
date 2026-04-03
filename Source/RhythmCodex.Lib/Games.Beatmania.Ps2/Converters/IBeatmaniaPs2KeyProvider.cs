using JetBrains.Annotations;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

[PublicAPI]
public interface IBeatmaniaPs2KeyProvider
{
    byte[] GetKeyFor14thStyle();
    byte[] GetKeyFor15thStyle();
    byte[] GetKeyFor16thStyle();
}