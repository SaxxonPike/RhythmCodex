using System.Collections.Generic;
using RhythmCodex.Infrastructure;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Bms.Converters;

public interface IBmsSoundLoader
{
    List<Sound?> Load(IDictionary<int, string> map, IFileAccessor accessor);
}