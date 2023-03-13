using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Bms.Converters;

public interface IBmsSoundLoader
{
    IList<ISound> Load(IDictionary<int, string> map, IFileAccessor accessor);
}