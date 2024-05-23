using RhythmCodex.Sounds.Models;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Converters;

public interface IXwbDecoder
{
    Sound? Decode(XwbSound sound);
}