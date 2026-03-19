using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Archs.Psx.Model;

[Model]
public class PsxBmDataKeysoundTable
{
    public List<PsxBmDataKeysoundTableInfo> Infos { get; set; } = [];
}

[Model]
public class PsxBmDataKeysoundTableInfo
{
    
}