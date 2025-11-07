using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Converters;

public interface IXwbDecoder
{
    Sound? Decode(XwbSound sound);
}